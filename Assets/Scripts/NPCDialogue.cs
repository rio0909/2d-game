using UnityEngine;

public class NPCDialogue : MonoBehaviour, IInteractable
{
    [Header("Who is this?")]
    public string npcName;
    public Sprite face; // <--- This was missing or not being sent!
    
    [Header("What do they say?")]
    [TextArea(3, 10)]
    public string[] lines;

    private SpriteRenderer sr;
    private Transform player;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    public void Interact()
    {
        // --- FEATURE 1: LOOK AT PLAYER ---
        if (player != null)
        {
            if (player.position.x < transform.position.x) sr.flipX = true; 
            else sr.flipX = false;
        }

        // --- FEATURE 2: USE REAL NAME ---
        string[] finalLines = new string[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            finalLines[i] = lines[i].Replace("{player}", GameData.playerName);
        }

        // --- FEATURE 3: SHOW UI (Now sending the FACE too!) ---
        // This is the line that fixed the red error:
        DialogueUI.Instance.Show(npcName, finalLines, face);

        // --- FEATURE 4: QUEST UNLOCK ---
        if (npcName == "Prof. Rio")
        {
            GameData.hasLabAccess = true;
            Debug.Log("Access Granted: Lab Unlocked!");
        }
    }
}