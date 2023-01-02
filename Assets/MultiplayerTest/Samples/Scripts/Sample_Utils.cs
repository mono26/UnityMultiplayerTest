using UnityEngine;

public class Sample_Utils
{
    public enum GameType : int
    {
        TeamDeathMatch = 0,
        CaptureTheFlag = 1,
        DeathMatch = 2,
        FreeForAll = 3,
        KingOfTheHill = 4
    }

    public enum GameMap : int
    {
        City = 0,
        Forest = 1,
        Desert = 2,
        Snow = 3,
        Space = 4
    }
}
