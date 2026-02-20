using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using UIInfoSuite2.Infrastructure;

namespace UIInfoSuite2.UIElements;

internal class LuckOfDay : IDisposable
{
#region Properties
  private readonly PerScreen<string> _hoverText = new(() => string.Empty);
  private readonly PerScreen<Color> _color = new(() => new Color(Color.White.ToVector4()));

  private readonly PerScreen<ClickableTextureComponent> _icon = new(
    () => new ClickableTextureComponent(
      "",
      new Rectangle(Tools.GetWidthInPlayArea() - 134, 290, 10 * Game1.pixelZoom, 10 * Game1.pixelZoom),
      "",
      "",
      Game1.mouseCursors,
      new Rectangle(50, 428, 10, 14),
      Game1.pixelZoom
    )
  );

  private readonly IModHelper _helper;

  private double _lastDailyLuck = double.NaN;
  private string? _cachedLuckText;
  private int _lastLuckCategory = -999;

  private bool Enabled { get; set; }
  private bool ShowExactValue { get; set; }

  private static readonly Color Luck1Color = new(87, 255, 106, 255);
  private static readonly Color Luck2Color = new(148, 255, 210, 255);
  private static readonly Color Luck3Color = new(246, 255, 145, 255);
  private static readonly Color Luck4Color = new(255, 255, 255, 255);
  private static readonly Color Luck5Color = new(255, 155, 155, 255);
  private static readonly Color Luck6Color = new(165, 165, 165, 204);
#endregion

#region Lifecycle
  public LuckOfDay(IModHelper helper)
  {
    _helper = helper;
  }

  public void Dispose()
  {
    ToggleOption(false);
  }

  public void ToggleOption(bool showLuckOfDay)
  {
    Enabled = showLuckOfDay;

    _helper.Events.Player.Warped -= OnWarped;
    _helper.Events.Display.RenderingHud -= OnRenderingHud;
    _helper.Events.Display.RenderedHud -= OnRenderedHud;
    _helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;

    if (showLuckOfDay)
    {
      AdjustIconXToBlackBorder();
      _helper.Events.Player.Warped += OnWarped;
      _helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
      _helper.Events.Display.RenderingHud += OnRenderingHud;
      _helper.Events.Display.RenderedHud += OnRenderedHud;
    }
  }

  public void ToggleShowExactValueOption(bool showExactValue)
  {
    ShowExactValue = showExactValue;
    ToggleOption(Enabled);
  }
#endregion

#region Event subscriptions
  private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
  {
    CalculateLuck(e);
  }

  private void OnRenderedHud(object sender, RenderedHudEventArgs e)
  {
    // draw hover text
    if (_icon.Value.containsPoint(Game1.getMouseX(), Game1.getMouseY()))
    {
      IClickableMenu.drawHoverText(Game1.spriteBatch, _hoverText.Value, Game1.dialogueFont);
    }
  }

  private void OnRenderingHud(object sender, RenderingHudEventArgs e)
  {
    // draw dice icon
    if (UIElementUtils.IsRenderingNormally())
    {
      Point iconPosition = IconHandler.Handler.GetNewIconPosition();
      ClickableTextureComponent icon = _icon.Value;
      icon.bounds.X = iconPosition.X;
      icon.bounds.Y = iconPosition.Y;
      _icon.Value = icon;
      _icon.Value.draw(Game1.spriteBatch, _color.Value, 1f);
    }
  }
#endregion

#region Logic
  private void CalculateLuck(UpdateTickedEventArgs e)
  {
    if (e.IsMultipleOf(30)) // half second
    {
      double luck = Game1.player.DailyLuck;
      int category = luck switch
      {
        > 0.07 => 1,
        > 0.02 => 2,
        0 => 4,
        >= -0.02 => 3,
        >= -0.07 => 5,
        _ => 6
      };

      if (category != _lastLuckCategory)
      {
        _lastLuckCategory = category;
        (_hoverText.Value, _color.Value) = category switch
        {
          1 => (I18n.LuckStatus1(), Luck1Color),
          2 => (I18n.LuckStatus2(), Luck2Color),
          3 => (I18n.LuckStatus3(), Luck3Color),
          4 => (I18n.LuckStatus4(), Luck4Color),
          5 => (I18n.LuckStatus5(), Luck5Color),
          _ => (I18n.LuckStatus6(), Luck6Color)
        };
      }

      // Rewrite the text, but keep the color
      if (ShowExactValue)
      {
        if (luck != _lastDailyLuck)
        {
          _lastDailyLuck = luck;
          _cachedLuckText = string.Format(I18n.DailyLuckValue(), luck.ToString("N3"));
        }
        _hoverText.Value = _cachedLuckText!;
      }
    }
  }

  private void OnWarped(object sender, WarpedEventArgs e)
  {
    // adjust icon X to black border
    if (e.IsLocalPlayer)
    {
      AdjustIconXToBlackBorder();
    }
  }

  private void AdjustIconXToBlackBorder()
  {
    _icon.Value = new ClickableTextureComponent(
      "",
      new Rectangle(Tools.GetWidthInPlayArea() - 134, 290, 10 * Game1.pixelZoom, 10 * Game1.pixelZoom),
      "",
      "",
      Game1.mouseCursors,
      new Rectangle(50, 428, 10, 14),
      Game1.pixelZoom
    );
  }
#endregion
}
