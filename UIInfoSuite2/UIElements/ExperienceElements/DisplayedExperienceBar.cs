using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
namespace UIInfoSuite2.UIElements.ExperienceElements;

public class DisplayedExperienceBar
{
  private const int MaxBarWidth = 220;

  public void Draw(
    Color experienceFillColor,
    Rectangle experienceIconPosition,
    int experienceEarnedThisLevel,
    int experienceDifferenceBetweenLevels,
    int currentLevel,
    Texture2D? iconTexture = null
  )
  {
    int barWidth = GetBarWidth(experienceEarnedThisLevel, experienceDifferenceBetweenLevels);
    Rectangle safeArea = Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea;
    float leftSide = GetExperienceBarLeftSide(safeArea);
    int bottom = safeArea.Bottom;

    Game1.drawDialogueBox(
      (int)leftSide,
      bottom - 160,
      285,
      160,
      false,
      true
    );

    Game1.spriteBatch.Draw(
      Game1.staminaRect,
      new Rectangle((int)leftSide + 32, bottom - 63, barWidth, 31),
      experienceFillColor
    );

    Game1.spriteBatch.Draw(
      Game1.staminaRect,
      new Rectangle(
        (int)leftSide + 32,
        bottom - 63,
        Math.Min(4, barWidth),
        31
      ),
      experienceFillColor
    );

    Game1.spriteBatch.Draw(
      Game1.staminaRect,
      new Rectangle((int)leftSide + 32, bottom - 63, barWidth, 4),
      experienceFillColor
    );

    Game1.spriteBatch.Draw(
      Game1.staminaRect,
      new Rectangle((int)leftSide + 32, bottom - 36, barWidth, 4),
      experienceFillColor
    );

    if (IsMouseOverExperienceBar(leftSide, bottom))
    {
      Game1.drawWithBorder(
        experienceEarnedThisLevel + "/" + experienceDifferenceBetweenLevels,
        Color.Black,
        Color.Black,
        new Vector2(leftSide + 33, bottom - 70)
      );
    }
    else
    {
      Game1.spriteBatch.Draw(
        iconTexture ?? Game1.mouseCursors,
        new Vector2(leftSide + 54, bottom - 62),
        experienceIconPosition,
        Color.White,
        0,
        Vector2.Zero,
        2.9f,
        SpriteEffects.None,
        0.85f
      );

      Game1.drawWithBorder(
        currentLevel.ToString(),
        Color.Black * 0.6f,
        Color.Black,
        new Vector2(leftSide + 33, bottom - 70)
      );
    }
  }

#region Static helpers
  private static int GetBarWidth(int experienceEarnedThisLevel, int experienceDifferenceBetweenLevels)
  {
    return (int)((double)experienceEarnedThisLevel / experienceDifferenceBetweenLevels * MaxBarWidth);
  }

  private static float GetExperienceBarLeftSide(Rectangle safeArea)
  {
    float leftSide = safeArea.Left;

    if (Game1.isOutdoorMapSmallerThanViewport())
    {
      int num3 = Game1.currentLocation.map.Layers[0].LayerWidth * Game1.tileSize;
      leftSide += (safeArea.Right - num3) / 2;
    }

    return leftSide;
  }

  private static bool IsMouseOverExperienceBar(float leftSide, int bottom)
  {
    return new Rectangle(
      (int)leftSide - 36,
      bottom - 80,
      305,
      100
    ).Contains(Game1.getMouseX(), Game1.getMouseY());
  }
#endregion
}
