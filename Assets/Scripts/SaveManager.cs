using UnityEngine;
using System.IO; 

[System.Serializable]
public class PlayerData
{
    public int totalCoins; 
}

public static class SaveManager
{
    public static void AddCoins(int amount)
    {
        PlayerData data = LoadData();
        data.totalCoins += amount;
        SaveFile(data);
    }

    public static int GetCoins()
    {
        return LoadData().totalCoins;
    }

    public static bool TrySpendCoins(int cost)
    {
        PlayerData data = LoadData();
        if (data.totalCoins >= cost)
        {
            data.totalCoins -= cost;
            SaveFile(data);
            return true;
        }
        return false;
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
        return new PlayerData(); 
    }
}