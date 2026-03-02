using UnityEngine;

public class NPCDialogue : MonoBehaviour, IInteractable
{
    [Header("Who is this?")]
    public string npcName;
    public Sprite face;
    
    [Header("What do they say?")]
    [TextArea(3, 10)]
    public string[] lines;

    [Header("Post-Conversation")]
    public bool stopAfterFirstTime = true;
    [TextArea(2, 5)] public string[] completedLines; 
    
    private bool hasFinishedMainDialogue = false;
    private int currentIndex = 0; 
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public int GetCurrentIndex() { return currentIndex; }
    public void SetCurrentIndex(int val) { currentIndex = val; }
    public void MarkAsComplete() { hasFinishedMainDialogue = true; }
    public bool HasFinished() { return hasFinishedMainDialogue; }
    
    public void Interact()
    {
        if (hasFinishedMainDialogue && stopAfterFirstTime)
        {
            if (completedLines.Length > 0)
            {
                DialogueUI.Instance.Show(npcName, completedLines, face, this);
            }
            return; 
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null && sr != null)
        {
            sr.flipX = (playerObj.transform.position.x < transform.position.x);
        }

        string[] finalLines = new string[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            finalLines[i] = lines[i].Replace("{player}", GameData.playerName);
        }

        DialogueUI.Instance.Show(npcName, finalLines, face, this);

        // Check Prof Logic ONLY if actually finished
        if (npcName == "Prof. Rio" && hasFinishedMainDialogue)
        {
            GameData.hasLabAccess = true;
            GameData.SaveGame();
        }
    }
}