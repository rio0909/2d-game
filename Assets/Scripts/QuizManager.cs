using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Video; 

public class QuizManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI scoreText;   
    public TextMeshProUGUI levelText;   
    public TextMeshProUGUI[] answerButtonTexts; 
    public GameObject[] answerButtons;           
    public GameObject[] livesIcons;             

    [Header("Level Settings")]
    public GameObject levelCompletePanel;    // Linked to Panel_LevelComplete
    public GameObject level2CompletePanel;   // Linked to Panel_Level2Complete
    public int questionsPerLevel = 5;           

    [Header("Video Screens")]
    public VideoPlayer winVideo;   
    public VideoPlayer loseVideo;  

    [Header("Game Settings")]
    public int maxLives = 3;
    private int currentLives;
    private Vector2[] initialPositions;

    [Header("Level 3 UI")]
    public TMP_InputField blankInput;    
    public GameObject submitBlankButton; 

    [Header("Level 2 Button Positions")] 
    public Vector2 trueButtonPos;   
    public Vector2 falseButtonPos;  

    private int currentScore = 0;       
    private int currentLevel = 1;       
    private int currentQuestionIndex = 0;
    private bool isQuizActive = true; 

    [System.Serializable]
    public class Question
    {
        [TextArea] public string questionText;
        public string[] answers;    
        public int correctAnswerID; 
    }
    public Question[] allQuestions;

    void Awake()
    {
        // 1. Snapshot the EXACT positions of the buttons before the game starts
        initialPositions = new Vector2[answerButtons.Length];
        for (int i = 0; i < answerButtons.Length; i++)
        {
            initialPositions[i] = answerButtons[i].GetComponent<RectTransform>().anchoredPosition;
        }
    }

    void Start()
    {
        RestartQuiz();
    }

    public void RestartQuiz()
    {
        isQuizActive = true;
        currentLives = maxLives;
        currentScore = 0;              // Reset Score
        currentQuestionIndex = 0;
        currentLevel = 1;              // Reset Level
        
        // Hide Screens
        if(winVideo != null) { winVideo.Stop(); winVideo.gameObject.SetActive(false); }
        if(loseVideo != null) { loseVideo.Stop(); loseVideo.gameObject.SetActive(false); }
        if(levelCompletePanel != null) levelCompletePanel.SetActive(false);
        if(level2CompletePanel != null) level2CompletePanel.SetActive(false); // <--- ADD THIS

        ResetLevel1Layout();
        UpdateUI();
        LoadQuestion();
    }

    void UpdateUI()
    {
        if(scoreText != null) scoreText.text = "SCORE: " + currentScore;
        if(levelText != null) levelText.text = "LEVEL: " + currentLevel;
        
        for (int i = 0; i < livesIcons.Length; i++)
        {
            if (livesIcons[i] != null) livesIcons[i].SetActive(i < currentLives);
        }
    }

    void LoadQuestion()
    {
        if (currentQuestionIndex < allQuestions.Length)
        {
            // --- LAYOUT MANAGER ---
            if (currentQuestionIndex < 5) // Questions 1-5: True/False
            {
                SetupLevel2Layout();
                blankInput.gameObject.SetActive(false);
                submitBlankButton.SetActive(false);
            }
            else if (currentQuestionIndex < 10) // Questions 6-10: Multiple Choice
            {
                ResetLevel1Layout();
                blankInput.gameObject.SetActive(false);
                submitBlankButton.SetActive(false);
            }
            else // Questions 11+: Fill in the Blanks
            {
                // Hide ALL multiple choice buttons
                foreach(GameObject btn in answerButtons) btn.SetActive(false);
                
                // Show Typing UI
                blankInput.gameObject.SetActive(true);
                submitBlankButton.SetActive(true);
                blankInput.text = ""; // Clear the box for the new question
            }

            // Display the text
            questionText.text = allQuestions[currentQuestionIndex].questionText;
            
            // Only update buttons if we are NOT in Level 3
            if (currentQuestionIndex < 10)
            {
                for (int i = 0; i < answerButtonTexts.Length; i++)
                {
                    if(i < allQuestions[currentQuestionIndex].answers.Length)
                        answerButtonTexts[i].text = allQuestions[currentQuestionIndex].answers[i];
                }
            }
        }
        else
        {
            isQuizActive = false; 
            if(winVideo != null) { winVideo.gameObject.SetActive(true); winVideo.Play(); }
        }
    }

    public void SubmitAnswer(int buttonID)
    {
        if (!isQuizActive) return;

        // Get the correct answer ID for the current question
        int correctID = allQuestions[currentQuestionIndex].correctAnswerID;

        // --- REPLACE EVERYTHING INSIDE THIS IF STATEMENT ---
        if (buttonID == correctID)
        {
            Debug.Log("Correct!");
            currentScore += 100; 
            currentQuestionIndex++; 

            // 1. Check if the WHOLE GAME is won (Question 15)
            if (currentQuestionIndex >= allQuestions.Length)
            {
                isQuizActive = false;
                if(levelCompletePanel != null) levelCompletePanel.SetActive(false);
                if(level2CompletePanel != null) level2CompletePanel.SetActive(false); 
                if(winVideo != null) { winVideo.gameObject.SetActive(true); winVideo.Play(); }
            }
            // 2. Show Level 1 Complete Panel (Question 5)
            else if (currentQuestionIndex == 5)
            {
                if(levelCompletePanel != null) levelCompletePanel.SetActive(true);
            }
            // 3. Show Level 2 Complete Panel (Question 10)
            else if (currentQuestionIndex == 10)
            {
                if(level2CompletePanel != null) level2CompletePanel.SetActive(true);
            }
            // 4. Otherwise, just load the next question
            else
            {
                LoadQuestion();
            }
        }
        // ---------------------------------------------------
        else
        {
            Debug.Log("Wrong!");
            currentLives--;
            UpdateUI();

            if (currentLives <= 0)
            {
                Debug.Log("GAME LOST!");
                isQuizActive = false;
                if(loseVideo != null) 
                {
                    loseVideo.gameObject.SetActive(true);
                    loseVideo.Play();
                }
            }
        }
        UpdateUI(); // Refresh score text
    }

    void OpenLevelComplete()
    {
        if(levelCompletePanel != null) levelCompletePanel.SetActive(true);
    }

   public void NextLevelButton()
    {
        // Hide both panels
        if(levelCompletePanel != null) levelCompletePanel.SetActive(false);
        if(level2CompletePanel != null) level2CompletePanel.SetActive(false);
        
        currentLevel++; 
        UpdateUI();     
        LoadQuestion();      
    }

    void ResetLevel1Layout()
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            // Turn the button on
            answerButtons[i].SetActive(true);

            // Move it back to the snapshot position we saved in Awake()
            if (initialPositions != null && i < initialPositions.Length)
            {
                answerButtons[i].GetComponent<RectTransform>().anchoredPosition = initialPositions[i];
            }
        }
    }

    void SetupLevel2Layout()
    {
        // 1. Hide buttons C and D
        answerButtons[2].SetActive(false);
        answerButtons[3].SetActive(false);

        // 2. Move A and B to the exact X and Y you type in the Inspector
        answerButtons[0].GetComponent<RectTransform>().anchoredPosition = trueButtonPos;
        answerButtons[1].GetComponent<RectTransform>().anchoredPosition = falseButtonPos;
    }
    
    public void SubmitTypedAnswer()
    {
        if (!isQuizActive) return;

        // Get what the user typed and what you put in Answers Element 0
        string userTyped = blankInput.text.Trim().ToLower();
        string correctAnswer = allQuestions[currentQuestionIndex].answers[0].Trim().ToLower();

        if (userTyped == correctAnswer)
        {
            currentScore += 150; // Bonus points for typing!
            currentQuestionIndex++;
            
            // Check for Win or Next Question
            if (currentQuestionIndex >= allQuestions.Length)
            {
                isQuizActive = false;
                if(winVideo != null) { winVideo.gameObject.SetActive(true); winVideo.Play(); }
            }
            else
            {
                LoadQuestion();
            }
        }
        else
        {
            currentLives--;
            UpdateUI();
            if (currentLives <= 0)
            {
                isQuizActive = false;
                if(loseVideo != null) { loseVideo.gameObject.SetActive(true); loseVideo.Play(); }
            }
        }
        UpdateUI();
    }
}