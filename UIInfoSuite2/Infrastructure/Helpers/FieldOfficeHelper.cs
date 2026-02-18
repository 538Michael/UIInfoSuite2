using StardewValley;
using StardewValley.Locations;

namespace UIInfoSuite2.Infrastructure.Helpers;

internal static class FieldOfficeHelper
{
  /// <summary>
  ///   Checks whether an item is still needed for donation at the Island Field Office (Professor Snail).
  ///   Returns false if Ginger Island hasn't been unlocked yet.
  /// </summary>
  public static bool IsItemNeededForFieldOffice(Item item)
  {
    if (item == null)
    {
      return false;
    }

    var office = Game1.getLocationFromName("IslandFieldOffice") as IslandFieldOffice;
    if (office == null)
    {
      return false;
    }

    // Logic mirrors FieldOfficeMenu.highlightBones() from the game code
    switch (item.QualifiedItemId)
    {
      case "(O)820":
        return !office.piecesDonated[5];
      case "(O)821":
        return !office.piecesDonated[4];
      case "(O)822":
        return !office.piecesDonated[3];
      case "(O)823":
        // Fossilized Leg is needed for two slots (pieces 0 and 2)
        return !office.piecesDonated[0] || !office.piecesDonated[2];
      case "(O)824":
        return !office.piecesDonated[1];
      case "(O)825":
        return !office.piecesDonated[8];
      case "(O)826":
        // Snake Vertebrae is needed for two slots (pieces 6 and 7)
        return !office.piecesDonated[7] || !office.piecesDonated[6];
      case "(O)827":
        return !office.piecesDonated[9];
      case "(O)828":
        return !office.piecesDonated[10];
      default:
        return false;
    }
  }
}
