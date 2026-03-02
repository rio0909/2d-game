using System.Collections;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }
    public bool isOpen { get; private set; } = false;

    [Header("UI References")]
    [SerializeField] private GameObject root;        
    [SerializeField] private Image portraitImage;    
    [SerializeField] private TextMeshProUGUI nameText; 
    [SerializeField] private TextMeshProUGUI bodyText;

    [Header("Typewriter Settings")]
    [SerializeField, Range(0.005f, 0.08f)] private float charDelay = 0.02f;

    [Header("Distance Settings")]
    [SerializeField] private float autoCloseDistance = 3.5f; 
    private Transform currentNPCTransform;
    private NPCDialogue currentNPC; 

    private string[] lines;
    private int index;
    private Coroutine typeRoutine;
    private bool isTyping;
    private string fullLine;
    private float openTime; 

    private void Awake()
    {
        Instance = this;
        if (root != null) root.SetActive(false);
    }

    private void Update()
    {
        if (!isOpen) return;

        if (currentNPCTransform != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float playerDist = Vector2.Distance(player.transform.position, currentNPCTransform.position);
                if (playerDist > autoCloseDistance)
                {
                    Hide();
                    return;
                }
            }
        }

        if (Time.time - openTime < 0.1f) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
        {
            if (isTyping) FinishLineInstant();
            else Next();
        }
    }

    public void Show(string speaker, string[] dialogueLines, Sprite face, NPCDialogue npcScript)
    {
        if (dialogueLines == null || dialogueLines.Length == 0) return;

        lines = dialogueLines;
        currentNPC = npcScript; 
        currentNPCTransform = npcScript.transform;
        
        // Get the last saved index
        index = npcScript.GetCurrentIndex(); 

        // --- THE FIX: If we walked away, move to the NEXT line when we come back ---
        // But only if we aren't at the very start or already past the end
        if (index > 0 && index < lines.Length && !npcScript.HasFinished())
        {
            // Optional: Remove this 'index++' if you want them to repeat the 
            // last line they were interrupted on. 
            // index++; 
            // npcScript.SetCurrentIndex(index);
        }

        if (index >= lines.Length)
        {
            Hide();
            return;
        }

        nameText.text = speaker;
        if (face != null)
        {
            portraitImage.sprite = face;
            portraitImage.gameObject.SetActive(true);
        }
        else portraitImage.gameObject.SetActive(false);

        openTime = Time.time; 
        root.SetActive(true);
        isOpen = true; 

        StartTyping(lines[index]);
    }

    private void Next()
    {
        index++;
        if (currentNPC != null) currentNPC.SetCurrentIndex(index);

        if (index >= lines.Length)
        {
            if (currentNPC != null) currentNPC.MarkAsComplete(); 
            Hide();
            return;
        }

        StartTyping(lines[index]);
    }

    private void StartTyping(string line)
    {
        if (typeRoutine != null) StopCoroutine(typeRoutine);
        fullLine = line;
        bodyText.text = ""; 
        typeRoutine = StartCoroutine(TypeLine(fullLine));
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        for (int i = 0; i < line.Length; i++)
        {
            bodyText.text += line[i];
            yield return new WaitForSeconds(charDelay);
        }
        isTyping = false;
        typeRoutine = null;
    }

    private void FinishLineInstant()
    {
        if (!isTyping) return;
        if (typeRoutine != null) StopCoroutine(typeRoutine);
        bodyText.text = fullLine; 
        isTyping = false;
        typeRoutine = null;
    }

    public void Hide()
    {
        isOpen = false;
        currentNPC = null;
        currentNPCTransform = null;
        if (root != null) root.SetActive(false);
        isTyping = false;
    }
}