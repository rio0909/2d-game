using System.Collections;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }

    // --- FLAG: Tells the PlayerInteract script if we are busy ---
    public bool isOpen { get; private set; } = false;

    [Header("UI References")]
    [SerializeField] private GameObject root;        // The Beige Panel
    [SerializeField] private Image portraitImage;    // The Face Image Slot
    [SerializeField] private TextMeshProUGUI nameText; 
    [SerializeField] private TextMeshProUGUI bodyText;

    [Header("Typewriter Settings")]
    [SerializeField, Range(0.005f, 0.08f)] private float charDelay = 0.02f;

    private string[] lines;
    private int index;
    private Coroutine typeRoutine;
    private bool isTyping;
    private string fullLine;

    // --- NEW: Timer to prevent "Instant Skip" bug ---
    private float openTime; 

    private void Awake()
    {
        Instance = this;
        if (root != null) root.SetActive(false);
    }

    private void Update()
    {
        // If the box is closed, stop listening for input
        if (!isOpen) return;

        // --- THE FIX: Ignore input for 0.1 seconds after opening ---
        // This prevents the "E" key you pressed to talk from skipping the text immediately.
        if (Time.time - openTime < 0.1f) return;

        // Press SPACE or E to advance
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
        {
            if (isTyping)
            {
                FinishLineInstant(); // Skip typing effect
            }
            else
            {
                Next(); // Go to next line
            }
        }
    }

    public void Show(string speaker, string[] dialogueLines, Sprite face)
    {
        if (dialogueLines == null || dialogueLines.Length == 0) return;

        lines = dialogueLines;
        index = 0;

        // 1. Set Name
        nameText.text = speaker;

        // 2. Set Face (If we have one)
        if (face != null)
        {
            portraitImage.sprite = face;
            portraitImage.gameObject.SetActive(true);
        }
        else
        {
            portraitImage.gameObject.SetActive(false);
        }

        // 3. Open UI & RECORD TIME
        openTime = Time.time; // <--- Mark the exact moment we opened it
        root.SetActive(true);
        isOpen = true; // Lock the player movement

        // 4. Start Typing
        StartTyping(lines[index]);
    }

    private void Next()
    {
        index++;
        if (index >= lines.Length)
        {
            Hide(); // End of conversation
            return;
        }

        StartTyping(lines[index]);
    }

    private void StartTyping(string line)
    {
        if (typeRoutine != null) StopCoroutine(typeRoutine);
        
        fullLine = line;
        bodyText.text = ""; // Clear text box
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

        bodyText.text = fullLine; // Show full text immediately
        isTyping = false;
        typeRoutine = null;
    }

    public void Hide()
    {
        isOpen = false; // Unlock the player
        if (root != null) root.SetActive(false);
        isTyping = false;
    }
}