using UnityEngine;
using System.IO; // Required for handling files

[System.Serializable]
public class PlayerData
{
    public int totalCoins; // This is the variable that gets saved
}

public static class SaveManager
{
    // Call this whenever you want to ADD money (e.g., SaveManager.AddCoins(100))
    public static void AddCoins(int amount)
    {
        // 1. Load existing data
        PlayerData data = LoadData();
        
        // 2. Add the new amount
        data.totalCoins += amount;
        
        // 3. Save it back to the file
        SaveFile(data);
        Debug.Log("Saved! Total Coins: " + data.totalCoins);
    }

    // Call this to get the current balance (e.g., int money = SaveManager.GetCoins())
    public static int GetCoins()
    {
        return LoadData().totalCoins;
    }

    private static void SaveFile(PlayerData data)
    {
        string json = JsonUtility.ToJson(data);
        string path = Application.persistentDataPath + "/playerdata.json";
        File.WriteAllText(path, json);
    }

    private static PlayerData LoadData()
    {
        string path = Application.persistentDataPath + "/playerdata.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<PlayerData>(json);
        }
        return new PlayerData(); // Return new empty data if file doesn't exist
    }
}