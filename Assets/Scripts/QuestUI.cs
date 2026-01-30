using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    public static QuestUI Instance { get; private set; }
    [SerializeField] private Text questText;

    private void Awake()
{
    Instance = this;
}

private void Start()
{
    Invoke(nameof(Refresh), 0.1f); // refresh after QuestManager initializes
}


    public static void RefreshIfExists()
    {
        if (Instance != null) Instance.Refresh();
    }

    public void Refresh()
    {
        if (questText == null || QuestManager.Instance == null) return;

        var sb = new StringBuilder();
        sb.AppendLine("Objectives:");

        bool any = false;
        foreach (var q in QuestManager.Instance.GetActive())
        {
            any = true;
            sb.AppendLine("• " + q);
        }
        if (!any) sb.AppendLine("• (none)");

        questText.text = sb.ToString();
    }
}
