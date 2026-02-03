using UnityEngine;

public static class GameData
{
    // --- VARIABLES TO SAVE ---
    public static string playerName = "Rio";
    public static bool hasLabAccess = false; 

    // --- SAVE FUNCTION ---
    public static void SaveGame()
    {
        PlayerPrefs.SetString("PlayerName", playerName);
        // We can't save 'bools' directly, so we use 1 for True, 0 for False
        PlayerPrefs.SetInt("HasLabAccess", hasLabAccess ? 1 : 0);
        
        PlayerPrefs.Save(); // Writes it to disk
        Debug.Log("Game Saved!");
    }

    // --- LOAD FUNCTION ---
    public static void LoadGame()
    {
        playerName = PlayerPrefs.GetString("PlayerName", "Rio");
        
        // 1 means True, 0 means False. Default is 0 (False).
        int labStatus = PlayerPrefs.GetInt("HasLabAccess", 0);
        hasLabAccess = (labStatus == 1);

        Debug.Log("Game Loaded! Lab Access: " + hasLabAccess);
    }
    
    // --- RESET FUNCTION (For Testing) ---
    public static void ResetData()
    {
        PlayerPrefs.DeleteAll();
        playerName = "Rio";
        hasLabAccess = false;
        Debug.Log("Data Wiped.");
    }
}