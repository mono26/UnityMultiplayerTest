using UnityEngine;

/// <summary>
/// Uility class used to store filters for matchmaking.
public class Sample_Utils
{
    /// <summary> Enum used to store the different game types. </summary>
    public enum GameType : int
    {
        TeamDeathMatch = 0,
        CaptureTheFlag = 1,
        DeathMatch = 2,
        FreeForAll = 3,
        KingOfTheHill = 4
    }

    /// <summary> Enum used to store the different maps. </summary>
    public enum GameMap : int
    {
        City = 0,
        Forest = 1,
        Desert = 2,
        Snow = 3,
        Space = 4
    }
}
