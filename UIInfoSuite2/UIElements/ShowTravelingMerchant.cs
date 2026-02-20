using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using UIInfoSuite2.Infrastructure;

namespace UIInfoSuite2.UIElements;

public class ShowTravelingMerchant : IDisposable
{
#region Properties
  private bool _travelingMerchantIsHere;
  private bool _travelingMerchantIsVisited;
  private readonly ClickableTextureComponent _travelingMerchantIcon = new(
    new Rectangle(0, 0, 40, 40), Game1.mouseCursors, new Rectangle(192, 1411, 20, 20), 2f);

  private bool Enabled { get; set; }
  private bool HideWhenVisited { get; set; }

  private readonly IModHelper _helper;
#endregion


#region Lifecycle
  public ShowTravelingMerchant(IModHelper helper)
  {
    _helper = helper;
  }

  public void Dispose()
  {
    ToggleOption(false);
  }

  public void ToggleOption(bool showTravelingMerchant)
  {
    Enabled = showTravelingMerchant;

    _helper.Events.Display.RenderingHud -= OnRenderingHud;
    _helper.Events.Display.RenderedHud -= OnRenderedHud;
    _helper.Events.GameLoop.DayStarted -= OnDayStarted;
    _helper.Events.Display.MenuChanged -= OnMenuChanged;

    if (showTravelingMerchant)
    {
      UpdateTravelingMerchant();
      _helper.Events.Display.RenderingHud += OnRenderingHud;
      _helper.Events.Display.RenderedHud += OnRenderedHud;
      _helper.Events.GameLoop.DayStarted += OnDayStarted;
      _helper.Events.Display.MenuChanged += OnMenuChanged;
    }
  }

  public void ToggleHideWhenVisitedOption(bool hideWhenVisited)
  {
    HideWhenVisited = hideWhenVisited;
    ToggleOption(Enabled);
  }
#endregion


#region Event subscriptions
  private void OnDayStarted(object sender, EventArgs e)
  {
    UpdateTravelingMerchant();
  }

  private void OnMenuChanged(object sender, MenuChangedEventArgs e)
  {
    if (e.NewMenu is ShopMenu menu && Game1.currentLocation.Name == "Forest")
    {
      foreach (ISalable item in menu.forSale)
      {
        if (item is not Hat)
        {
          _travelingMerchantIsVisited = true;
          break;
        }
      }
    }
  }

  private void OnRenderingHud(object sender, RenderingHudEventArgs e)
  {
    // Draw icon
    if (UIElementUtils.IsRenderingNormally() && ShouldDrawIcon())
    {
      UIElementUtils.DrawAtNextIconPosition(_travelingMerchantIcon);
    }
  }

  private void OnRenderedHud(object sender, RenderedHudEventArgs e)
  {
    // Show text on hover
    if (ShouldDrawIcon())
    {
      UIElementUtils.DrawHoverTextOnCursor(_travelingMerchantIcon, I18n.TravelingMerchantIsInTown());
    }
  }
#endregion


#region Logic
  private void UpdateTravelingMerchant()
  {
    _travelingMerchantIsHere = ((Forest)Game1.getLocationFromName(nameof(Forest))).ShouldTravelingMerchantVisitToday();
    _travelingMerchantIsVisited = false;
  }

  private bool ShouldDrawIcon()
  {
    return _travelingMerchantIsHere && (!_travelingMerchantIsVisited || !HideWhenVisited);
  }
#endregion
}
