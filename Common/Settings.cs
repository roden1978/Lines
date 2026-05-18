using System;

public static class Settings
{
    public const int ScreenWidth = 1200;
    public const int ScreenHeight = 740;
    public const int TilesCount = 9;
    public const int RandomTilesCount = 3;
    public const int WinCount = 5;
    public const int TilesWidth = 64;
    public const int TilesHeight = 1;
    public const string GameName = "LNS";
    public static string PlayerName => Environment.UserName;
    public const int LeaderboardCapacity = 5;
}