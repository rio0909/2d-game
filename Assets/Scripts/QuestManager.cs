using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    private readonly HashSet<string> active = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddQuest(string questId)
    {
        active.Add(questId);
        QuestUI.RefreshIfExists();
    }

    public void CompleteQuest(string questId)
    {
        active.Remove(questId);
        QuestUI.RefreshIfExists();
    }

    public IEnumerable<string> GetActive() => active;
}
