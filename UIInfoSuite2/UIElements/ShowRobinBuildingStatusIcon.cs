using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using UIInfoSuite2.Infrastructure;

namespace UIInfoSuite2.UIElements;

internal class ShowRobinBuildingStatusIcon : IDisposable
{
#region Properties
  private bool _IsBuildingInProgress;
  private static readonly Rectangle BuildingIconSpriteLocation = new(0, 196, 15, 14);
  private string _hoverText;
  private readonly PerScreen<ClickableTextureComponent> _buildingIcon = new(
    () => new ClickableTextureComponent(
      new Rectangle(0, 0, 40, 40), Game1.mouseCursors, Rectangle.Empty, 8 / 3f));
  private Texture2D _robinIconSheet;

  private readonly IModHelper _helper;
#endregion

#region Lifecycle
  public ShowRobinBuildingStatusIcon(IModHelper helper)
  {
    _helper = helper;
  }

  public void Dispose()
  {
    ToggleOption(false);
  }

  public void ToggleOption(bool showRobinBuildingStatus)
  {
    _helper.Events.GameLoop.DayStarted -= OnDayStarted;
    _helper.Events.Display.RenderingHud -= OnRenderingHud;
    _helper.Events.Display.RenderedHud -= OnRenderedHud;
    _helper.Events.GameLoop.OneSecondUpdateTicked -= OnTickInRobinHouse;

    if (showRobinBuildingStatus)
    {
      UpdateRobinBuindingStatusData();

      _helper.Events.GameLoop.DayStarted += OnDayStarted;
      _helper.Events.Display.RenderingHud += OnRenderingHud;
      _helper.Events.Display.RenderedHud += OnRenderedHud;
      _helper.Events.GameLoop.OneSecondUpdateTicked += OnTickInRobinHouse;
    }
  }
#endregion

#region Event subscriptions
  public void OnTickInRobinHouse(object? sender, OneSecondUpdateTickedEventArgs e)
  {
    if (Game1.currentLocation?.Name != "ScienceHouse")
    {
      return;
    }

    UpdateRobinBuindingStatusData();
  }

  private void OnDayStarted(object sender, DayStartedEventArgs e)
  {
    UpdateRobinBuindingStatusData();
  }

  private void OnRenderingHud(object sender, RenderingHudEventArgs e)
  {
    // Draw icon
    if (UIElementUtils.IsRenderingNormally() && _IsBuildingInProgress && _robinIconSheet != null)
    {
      _buildingIcon.Value.texture = _robinIconSheet;
      _buildingIcon.Value.sourceRect = BuildingIconSpriteLocation;
      UIElementUtils.DrawAtNextIconPosition(_buildingIcon.Value);
    }
  }

  private void OnRenderedHud(object sender, RenderedHudEventArgs e)
  {
    // Show text on hover
    if (_IsBuildingInProgress)
    {
      UIElementUtils.DrawHoverTextOnCursor(_buildingIcon.Value, _hoverText);
    }
  }
#endregion

#region Logic
  private bool GetRobinMessage(out string hoverText)
  {
    int remainingDays = Game1.player.daysUntilHouseUpgrade.Value;

    if (remainingDays <= 0)
    {
      Building? building = Game1.GetBuildingUnderConstruction();

      if (building is not null)
      {
        if (building.daysOfConstructionLeft.Value > building.daysUntilUpgrade.Value)
        {
          hoverText = string.Format(I18n.RobinBuildingStatus(), building.daysOfConstructionLeft.Value);
          return true;
        }

        // Add another translation string for this?
        hoverText = string.Format(I18n.RobinBuildingStatus(), building.daysUntilUpgrade.Value);
        return true;
      }

      hoverText = string.Empty;
      return false;
    }

    hoverText = string.Format(I18n.RobinHouseUpgradeStatus(), remainingDays);
    return true;
  }

  private void UpdateRobinBuindingStatusData()
  {
    if (GetRobinMessage(out _hoverText))
    {
      _IsBuildingInProgress = true;
      FindRobinSpritesheet();
    }
    else
    {
      _IsBuildingInProgress = false;
    }
  }

  private void FindRobinSpritesheet()
  {
    if (_robinIconSheet != null)
    {
      return;
    }

    Texture2D? foundTexture = Game1.getCharacterFromName("Robin")?.Sprite?.Texture;
    if (foundTexture != null)
    {
      _robinIconSheet = foundTexture;
    }
    else
    {
      ModEntry.MonitorObject.LogOnce($"{GetType().Name}: Could not find Robin spritesheet.", LogLevel.Warn);
    }
  }
#endregion
}
