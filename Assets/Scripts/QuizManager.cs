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

    [Header("Timer Settings")]
    public TextMeshProUGUI timerText;    // Drag 'Txt_Timer' (the numbers) here
    public GameObject timerLabelObject;  // <--- NEW: Drag 'Text_TIME' (the word) here
    public float timePerQuestion = 15f;  
    public float timeLevel3 = 10f;       
    private float currentTimer;     

    [Header("Level Settings")]
    public GameObject levelCompletePanel;    
    public GameObject level2CompletePanel;   
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

    void Update()
    {
        // 1. Only run if game is active AND we are on Level 2 or higher
        if (isQuizActive && currentLevel >= 2)
        {
            currentTimer -= Time.deltaTime; // Count down

            // 2. SHOW THE TIMER & LABEL
            if (timerText != null)
            {
                timerText.gameObject.SetActive(true);
                timerText.text = Mathf.CeilToInt(currentTimer).ToString();
                
                // Turn RED if under 5 seconds
                if (currentTimer <= 5) timerText.color = Color.red;
                else timerText.color = Color.white;
            }

            // --- NEW: Also show the "TIME" label ---
            if (timerLabelObject != null)
            {
                timerLabelObject.SetActive(true);
            }
            // ---------------------------------------

            // 3. TIME RAN OUT -> INSTANT GAME OVER
            if (currentTimer <= 0)
            {
                Debug.Log("Time's Up! Game Over.");
                
                isQuizActive = false; 

                if (loseVideo != null) { loseVideo.gameObject.SetActive(true); loseVideo.Play(); }

                // Hide timer and label
                if (timerText != null) timerText.gameObject.SetActive(false);
                if (timerLabelObject != null) timerLabelObject.SetActive(false);
            }
        }
        else
        {
            // --- HIDE EVERYTHING IN LEVEL 1 ---
            if (timerText != null) timerText.gameObject.SetActive(false);
            if (timerLabelObject != null) timerLabelObject.SetActive(false);
        }
    }

    public void RestartQuiz()
    {
        isQuizActive = true;
        currentLives = maxLives;
        currentScore = 0;              
        currentQuestionIndex = 0;
        currentLevel = 1;              
        
        if(winVideo != null) { winVideo.Stop(); winVideo.gameObject.SetActive(false); }
        if(loseVideo != null) { loseVideo.Stop(); loseVideo.gameObject.SetActive(false); }
        if(levelCompletePanel != null) levelCompletePanel.SetActive(false);
        if(level2CompletePanel != null) level2CompletePanel.SetActive(false);

        ResetLevel1Layout();
        UpdateUI();
        LoadQuestion();
    }

    void LoadQuestion()
    {
        if (currentQuestionIndex < allQuestions.Length)
        {
            ResetTimer();

            if (currentQuestionIndex < 5) 
            {
                SetupLevel2Layout();
                blankInput.gameObject.SetActive(false);
                submitBlankButton.SetActive(false);
            }
            else if (currentQuestionIndex < 10) 
            {
                ResetLevel1Layout();
                blankInput.gameObject.SetActive(false);
                submitBlankButton.SetActive(false);
            }
            else 
            {
                foreach(GameObject btn in answerButtons) btn.SetActive(false);
                blankInput.gameObject.SetActive(true);
                submitBlankButton.SetActive(true);
                blankInput.text = ""; 
            }

            questionText.text = allQuestions[currentQuestionIndex].questionText;
            
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

        ResetTimer();

        int correctID = allQuestions[currentQuestionIndex].correctAnswerID;

        if (buttonID == correctID)
        {
            Debug.Log("Correct!");

            int pointsEarned = 100;
            currentScore += pointsEarned;
            SaveManager.AddCoins(pointsEarned);

            currentQuestionIndex++;

            if (currentQuestionIndex >= allQuestions.Length)
            {
                isQuizActive = false;
                CheckAndQueueBadges();
                if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
                if (level2CompletePanel != null) level2CompletePanel.SetActive(false);
                if (winVideo != null) { winVideo.gameObject.SetActive(true); winVideo.Play(); }
            }
            else if (currentQuestionIndex == 5)
            {
                if (levelCompletePanel != null) levelCompletePanel.SetActive(true);
            }
            else if (currentQuestionIndex == 10)
            {
                if (level2CompletePanel != null) level2CompletePanel.SetActive(true);
            }
            else
            {
                LoadQuestion();
            }
        }
        else
        {
            Debug.Log("Wrong!");
            currentLives--;
            UpdateUI();

            if (currentLives <= 0)
            {
                isQuizActive = false;
                if (loseVideo != null) { loseVideo.gameObject.SetActive(true); loseVideo.Play(); }
            }
        }
        UpdateUI();
    }

    void OpenLevelComplete()
    {
        if(levelCompletePanel != null) levelCompletePanel.SetActive(true);
    }

    public void NextLevelButton()
    {
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
            answerButtons[i].SetActive(true);
            if (initialPositions != null && i < initialPositions.Length)
            {
                answerButtons[i].GetComponent<RectTransform>().anchoredPosition = initialPositions[i];
            }
        }
    }

    void SetupLevel2Layout()
    {
        answerButtons[2].SetActive(false);
        answerButtons[3].SetActive(false);
        answerButtons[0].GetComponent<RectTransform>().anchoredPosition = trueButtonPos;
        answerButtons[1].GetComponent<RectTransform>().anchoredPosition = falseButtonPos;
    }
    
    public void SubmitTypedAnswer()
    {
        if (!isQuizActive) return;

        ResetTimer();

        string userTyped = blankInput.text.Trim().ToLower();
        string correctAnswer = allQuestions[currentQuestionIndex].answers[0].Trim().ToLower();

        if (userTyped == correctAnswer)
        {
            int pointsEarned = 150;
            currentScore += pointsEarned;
            SaveManager.AddCoins(pointsEarned);

            currentQuestionIndex++;

            if (currentQuestionIndex >= allQuestions.Length)
            {
                isQuizActive = false;
                CheckAndQueueBadges();
                if (winVideo != null) { winVideo.gameObject.SetActive(true); winVideo.Play(); }
            }
            else
            {
                LoadQuestion();
            }
        }
        else
        {
            Debug.Log("Wrong!"); 
            currentLives--;
            UpdateUI();
            if (currentLives <= 0)
            {
                isQuizActive = false;
                if (loseVideo != null) { loseVideo.gameObject.SetActive(true); loseVideo.Play(); }
            }
        }
        UpdateUI();
    }
    
    void CheckAndQueueBadges()
    {
        BadgeManager bm = FindObjectOfType<BadgeManager>();
        if (bm == null) return;

        bm.UnlockBadge("QUIZ_COMPLETE");

        if (currentLives == maxLives)
        {
            bm.UnlockBadge("QUIZ_PERFECT");
        }

        if (currentLives == 1)
        {
            bm.UnlockBadge("QUIZ_SURVIVOR");
        }
    }

    void ResetTimer()
    {
        if (currentLevel == 3) currentTimer = timeLevel3;
        else currentTimer = timePerQuestion;
    }

    void UpdateUI()
    {
        if(scoreText != null) scoreText.text = "= " + currentScore;
        if(levelText != null) levelText.text = "LEVEL: " + currentLevel;
        
        for (int i = 0; i < livesIcons.Length; i++)
        {
            if (livesIcons[i] != null) livesIcons[i].SetActive(i < currentLives);
        }
    }
}