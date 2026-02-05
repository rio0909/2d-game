using UnityEngine;

public class NPCDialogue : MonoBehaviour, IInteractable
{
    [Header("Who is this?")]
    public string npcName;
    public Sprite face;
    
    [Header("What do they say?")]
    [TextArea(3, 10)]
    public string[] lines;

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    
    public void Interact()
    {
        // 1. FIND THE PLAYER RIGHT NOW (Fresh check)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        Transform playerTransform = playerObj != null ? playerObj.transform : null;

        // 2. LOOK AT PLAYER
        if (playerTransform != null)
        {
            // If player is to the LEFT, face LEFT
            if (playerTransform.position.x < transform.position.x) 
            {
                sr.flipX = true; 
            }
            // If player is to the RIGHT, face RIGHT
            else 
            {
                sr.flipX = false;
            }
        }

        // 3. PREPARE LINES (Replace {player} with real name)
        string[] finalLines = new string[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            finalLines[i] = lines[i].Replace("{player}", GameData.playerName);
        }

        // 4. SHOW DIALOGUE UI
        DialogueUI.Instance.Show(npcName, finalLines, face);

        // --- SPECIAL NPC LOGIC ---
        
        // Logic for Prof. Rio
        if (npcName == "Prof. Rio")
        {
            GameData.hasLabAccess = true;
            GameData.SaveGame();
            Debug.Log("Access Granted: Lab Unlocked!");
        }

        // Logic for Prof. Jessi (New!)
        if (npcName == "Prof. Jessi")
        {
            Debug.Log("Prof. Jessi is teaching class.");
            // Later you can add: GameData.hasAttendedClass = true;
        }
    }
}