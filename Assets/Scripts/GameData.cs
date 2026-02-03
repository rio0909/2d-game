public static class GameData
{
    // This variable stays alive correctly everywhere
    public static string playerName = "Rookie";
    // This is the missing line that caused the error!
    public static bool hasLabAccess = false;
}