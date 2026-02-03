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
    // We don't save 'player' here anymore to avoid stale references

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
            // Debug line to prove it found the player
            Debug.Log("Looking at Player!"); 

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
        else
        {
            Debug.LogError("COULD NOT FIND PLAYER! Check your Player Tag!");
        }

        // 3. DO THE REST (Dialogue code...)
        string[] finalLines = new string[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            finalLines[i] = lines[i].Replace("{player}", GameData.playerName);
        }

        DialogueUI.Instance.Show(npcName, finalLines, face);

        if (npcName == "Prof. Rio")
        {
            GameData.hasLabAccess = true;
        }
    }
}