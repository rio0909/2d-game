using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject root;
    [SerializeField] private Text nameText;
    [SerializeField] private Text bodyText;

    [Header("Typewriter Settings")]
    [SerializeField, Range(0.005f, 0.08f)] private float charDelay = 0.02f;

    [Header("Choices")]
    [SerializeField] private GameObject choicesRoot;      // Assign your "Choices" GameObject here
    [SerializeField] private Button choiceButtonPrefab;   // Assign your ChoiceButton prefab here

    private string[] lines;
    private int index;

    private Coroutine typeRoutine;
    private bool isTyping;
    private string fullLine;

    private DialogueChoice[] currentChoices;

    private void Awake()
    {
        Instance = this;

        if (root != null) root.SetActive(false);
        if (choicesRoot != null) choicesRoot.SetActive(false);
    }

    private void Update()
    {
        if (root == null || !root.activeSelf) return;

        // Space behavior:
        // - If typing: instantly finish current line
        // - If not typing: go to next line (only if choices are not open)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                FinishLineInstant();
            }
            else
            {
                // If choices are visible, don't advance lines on Space (prevents skipping)
                if (choicesRoot == null || !choicesRoot.activeSelf)
                    Next();
            }
        }

        // Choice hotkeys (1/2/3)
        if (choicesRoot != null && choicesRoot.activeSelf && currentChoices != null)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) SelectChoice(0);
            if (Input.GetKeyDown(KeyCode.Alpha2) && currentChoices.Length > 1) SelectChoice(1);
            if (Input.GetKeyDown(KeyCode.Alpha3) && currentChoices.Length > 2) SelectChoice(2);
        }

        if (Input.GetKeyDown(KeyCode.Escape)) Hide();
    }

    public void Show(string speaker, string[] dialogueLines)
    {
        if (dialogueLines == null || dialogueLines.Length == 0) return;

        lines = dialogueLines;
        index = 0;

        nameText.text = speaker;
        root.SetActive(true);

        // Ensure choices are hidden when starting a new dialogue
        HideChoices();

        StartTyping(lines[index]);
    }

    private void Next()
    {
        index++;
        if (lines == null || index >= lines.Length)
        {
            Hide();
            return;
        }

        StartTyping(lines[index]);
    }

    private void StartTyping(string line)
    {
        // Stop any previous typewriter coroutine
        if (typeRoutine != null)
            StopCoroutine(typeRoutine);

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

        if (typeRoutine != null)
            StopCoroutine(typeRoutine);

        bodyText.text = fullLine;
        isTyping = false;
        typeRoutine = null;
    }

    // -----------------------
    // Choices
    // -----------------------
    public void ShowChoices(DialogueChoice[] choices)
    {
        if (choicesRoot == null || choiceButtonPrefab == null)
        {
            Debug.LogWarning("DialogueUI: choicesRoot or choiceButtonPrefab is not assigned in the Inspector.");
            return;
        }

        if (choices == null || choices.Length == 0)
        {
            HideChoices();
            return;
        }

        currentChoices = choices;
        choicesRoot.SetActive(true);

        // Clear old buttons
        foreach (Transform child in choicesRoot.transform)
            Destroy(child.gameObject);

        // Create buttons
        for (int i = 0; i < choices.Length; i++)
        {
            int idx = i;

            Button btn = Instantiate(choiceButtonPrefab, choicesRoot.transform);

            Text label = btn.GetComponentInChildren<Text>();
            if (label != null)
                label.text = $"{i + 1}. {choices[i].text}";

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => SelectChoice(idx));
        }
    }

    private void SelectChoice(int choiceIndex)
    {
        if (currentChoices == null) return;
        if (choiceIndex < 0 || choiceIndex >= currentChoices.Length) return;

        var chosen = currentChoices[choiceIndex];

        HideChoices();

        // Run the choice action
        chosen?.onSelect?.Invoke();
    }

    private void HideChoices()
    {
        currentChoices = null;

        if (choicesRoot != null)
        {
            choicesRoot.SetActive(false);

            // Optional: clear buttons so old ones don't flash next time
            foreach (Transform child in choicesRoot.transform)
                Destroy(child.gameObject);
        }
    }

    // -----------------------
    // Close
    // -----------------------
    public void Hide()
    {
        // Stop typewriter if it was running
        if (typeRoutine != null)
            StopCoroutine(typeRoutine);

        isTyping = false;
        typeRoutine = null;

        HideChoices();

        if (root != null) root.SetActive(false);

        lines = null;
        index = 0;

        // Optional: clear text when closing
        if (bodyText != null) bodyText.text = "";
    }
}
