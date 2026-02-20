using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using UIInfoSuite2.Infrastructure;

namespace UIInfoSuite2.UIElements;

public static class UIElementUtils
{
  public const float DefaultIconScale = 8 / 3f;

  public static bool IsRenderingNormally()
  {
    return !Game1.game1.takingMapScreenshot &&
           !Game1.eventUp &&
           !Game1.viewportFreeze &&
           !Game1.freezeControls &&
           Game1.viewportHold <= 0 &&
           Game1.displayHUD;
  }

  public static void DrawAtNextIconPosition(ClickableTextureComponent icon)
  {
    Point pos = IconHandler.Handler.GetNewIconPosition();
    icon.bounds.X = pos.X;
    icon.bounds.Y = pos.Y;
    icon.draw(Game1.spriteBatch);
  }

  public static void DrawAtNextIconPosition(ClickableTextureComponent icon, Color color, float layerDepth)
  {
    Point pos = IconHandler.Handler.GetNewIconPosition();
    icon.bounds.X = pos.X;
    icon.bounds.Y = pos.Y;
    icon.draw(Game1.spriteBatch, color, layerDepth);
  }

  public static void DrawHoverTextOnCursor(ClickableTextureComponent? icon, string? hoverText)
  {
    if (string.IsNullOrEmpty(hoverText) ||
        icon is null ||
        !icon.containsPoint(Game1.getMouseX(), Game1.getMouseY()))
    {
      return;
    }

    IClickableMenu.drawHoverText(Game1.spriteBatch, hoverText, Game1.dialogueFont);
  }

  public static void DrawTextWithShadow(
    SpriteBatch b,
    SpriteFont font,
    string text,
    Vector2 position,
    Color textColor,
    Color shadowColor,
    Vector2 shadowOffset)
  {
    b.DrawString(font, text, position + shadowOffset, shadowColor);
    b.DrawString(font, text, position, textColor);
  }
}
