using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Video; 
using System.Collections; 

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
    public TextMeshProUGUI timerText;    
    public GameObject timerLabelObject;  
    public float timePerQuestion = 15f;  
    public float timeLevel3 = 10f;       
    private float currentTimer;     

    [Header("Level Settings")]
    public GameObject levelCompletePanel;    
    public GameObject level2CompletePanel;   
    public int questionsPerLevel = 5;          

    [Header("Final Results UI")]
    public GameObject finalResultsPanel;     
    public TextMeshProUGUI finalCorrectText; 
    public TextMeshProUGUI finalScoreText;   
    public GameObject retryButton; 
    
    [Header("Animation Settings")]
    public float textTypingSpeed = 0.05f;  

    [Header("Video Screens")]
    public VideoPlayer winVideo;   
    public VideoPlayer loseVideo;  
 
    [Header("Game Settings")]
    public float colorFlashDelay = 0.4f; 
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
        if (isQuizActive && currentLevel >= 2)
        {
            currentTimer -= Time.deltaTime; 

            if (timerText != null)
            {
                timerText.gameObject.SetActive(true);
                timerText.text = Mathf.CeilToInt(currentTimer).ToString();
                
                if (currentTimer <= 5) timerText.color = Color.red;
                else timerText.color = Color.white;
            }

            if (timerLabelObject != null)
            {
                timerLabelObject.SetActive(true);
            }

            if (currentTimer <= 0)
            {
                Debug.Log("Time's Up! Game Over.");
                isQuizActive = false; 
                TriggerEndGame(false); 
            }
        }
        else
        {
            if (timerText != null) timerText.gameObject.SetActive(false);
            if (timerLabelObject != null) timerLabelObject.SetActive(false);
        }
    }

    public void RestartQuiz()
    {
        StopAllCoroutines(); 
        ShowMainQuizUI();
        
        isQuizActive = true;
        currentLives = maxLives;
        currentScore = 0;              
        currentQuestionIndex = 0;
        currentLevel = 1;              
        
        if(winVideo != null) { winVideo.Stop(); winVideo.gameObject.SetActive(false); }
        if(loseVideo != null) { loseVideo.Stop(); loseVideo.gameObject.SetActive(false); }
        if(levelCompletePanel != null) levelCompletePanel.SetActive(false);
        if(level2CompletePanel != null) level2CompletePanel.SetActive(false);
        if(finalResultsPanel != null) finalResultsPanel.SetActive(false); 

        ResetLevel1Layout();
        UpdateUI();
        LoadQuestion();
    }

    void LoadQuestion()
    {
        foreach(GameObject btn in answerButtons)
        {
            btn.GetComponent<Image>().color = Color.white;
        }
        if (blankInput != null) blankInput.GetComponent<Image>().color = Color.white;

        if (currentQuestionIndex < allQuestions.Length)
        {
            ResetTimer();
            isQuizActive = true; 

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
            TriggerEndGame(true); 
        }
    }

    public void SubmitAnswer(int buttonID)
    {
        if (!isQuizActive) return;
        
        int correctID = allQuestions[currentQuestionIndex].correctAnswerID;
        bool isCorrect = (buttonID == correctID);

        StartCoroutine(ShowButtonColorAndProcess(buttonID, isCorrect));
    }

    IEnumerator ShowButtonColorAndProcess(int buttonID, bool isCorrect)
    {
        isQuizActive = false; 

        Image btnImage = answerButtons[buttonID].GetComponent<Image>();
        if (btnImage != null)
        {
            btnImage.color = isCorrect ? Color.green : Color.red;
        }

        yield return new WaitForSeconds(colorFlashDelay);

        ProcessAnswerLogic(isCorrect);
    }

    public void SubmitTypedAnswer()
    {
        if (!isQuizActive) return;

        string userTyped = blankInput.text.Trim().ToLower();
        string correctAnswer = allQuestions[currentQuestionIndex].answers[0].Trim().ToLower();
        bool isCorrect = (userTyped == correctAnswer);

        StartCoroutine(ShowInputColorAndProcess(isCorrect));
    }

    IEnumerator ShowInputColorAndProcess(bool isCorrect)
    {
        isQuizActive = false;

        Image inputImage = blankInput.GetComponent<Image>();
        if (inputImage != null)
        {
            inputImage.color = isCorrect ? Color.green : Color.red;
        }

        yield return new WaitForSeconds(colorFlashDelay);

        ProcessAnswerLogic(isCorrect);
    }

    void ProcessAnswerLogic(bool isCorrect)
    {
        ResetTimer();

        if (isCorrect)
        {
            int pointsEarned = (currentLevel == 3) ? 150 : 100; 
            currentScore += pointsEarned;
            SaveManager.AddCoins(pointsEarned);

            currentQuestionIndex++;

            if (currentQuestionIndex >= allQuestions.Length)
            {
                isQuizActive = false;
                CheckAndQueueBadges();
                
                if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
                if (level2CompletePanel != null) level2CompletePanel.SetActive(false);
                
                TriggerEndGame(true); 
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
            currentLives--;
            UpdateUI();

            if (currentLives <= 0)
            {
                isQuizActive = false;
                TriggerEndGame(false); 
            }
            else
            {
                 LoadQuestion(); 
            }
        }
        UpdateUI();
    }

    void TriggerEndGame(bool isWin)
    {
        HideMainQuizUI();

        VideoPlayer activeVideo = isWin ? winVideo : loseVideo;

        if (activeVideo != null)
        {
            activeVideo.gameObject.SetActive(true);
            activeVideo.Play();
        }

        ShowFinalResults(isWin);
    }

    void ShowFinalResults(bool isWin)
    {
        if (finalResultsPanel != null) 
        {
            string correctString = "Questions Correct: " + currentQuestionIndex + " / " + allQuestions.Length;
            string scoreString = "Final Score: " + currentScore;
            
            StartCoroutine(AnimateResultsScreen(correctString, scoreString, isWin));
        }
    }

    IEnumerator AnimateResultsScreen(string correctStr, string scoreStr, bool isWin)
    {
        finalResultsPanel.SetActive(true);

        if (retryButton != null)
        {
            retryButton.SetActive(!isWin); 
        }

        // --- NEW: Change the text color based on winning or losing! ---
        Color resultColor = isWin ? Color.green : Color.red;

        if (finalCorrectText != null) 
        { 
            finalCorrectText.color = resultColor;
            finalCorrectText.text = correctStr; 
            finalCorrectText.maxVisibleCharacters = 0; 
        }

        if (finalScoreText != null) 
        { 
            finalScoreText.color = resultColor;
            finalScoreText.text = scoreStr; 
            finalScoreText.maxVisibleCharacters = 0; 
        }
        // --------------------------------------------------------------

        if (finalCorrectText != null)
        {
            for (int i = 0; i <= correctStr.Length; i++)
            {
                finalCorrectText.maxVisibleCharacters = i;
                yield return new WaitForSeconds(textTypingSpeed);
            }
        }

        yield return new WaitForSeconds(0.2f); 

        if (finalScoreText != null)
        {
            for (int i = 0; i <= scoreStr.Length; i++)
            {
                finalScoreText.maxVisibleCharacters = i;
                yield return new WaitForSeconds(textTypingSpeed);
            }
        }
    }

    void HideMainQuizUI()
    {
        if (questionText != null) questionText.gameObject.SetActive(false);
        if (scoreText != null) scoreText.gameObject.SetActive(false);
        if (levelText != null) levelText.gameObject.SetActive(false);
        if (timerText != null) timerText.gameObject.SetActive(false);
        if (timerLabelObject != null) timerLabelObject.SetActive(false);
        if (blankInput != null) blankInput.gameObject.SetActive(false);
        if (submitBlankButton != null) submitBlankButton.SetActive(false);
        
        foreach (GameObject btn in answerButtons) { if (btn != null) btn.SetActive(false); }
        foreach (GameObject icon in livesIcons) { if (icon != null) icon.SetActive(false); }
    }

    void ShowMainQuizUI()
    {
        if (questionText != null) questionText.gameObject.SetActive(true);
        if (scoreText != null) scoreText.gameObject.SetActive(true);
        if (levelText != null) levelText.gameObject.SetActive(true);
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