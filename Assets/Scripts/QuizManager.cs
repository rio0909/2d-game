using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Video; // Essential for video playback

public class QuizManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI[] answerButtonTexts; // Drag your 4 button texts here
    public GameObject[] livesIcons;             // Drag Heart1, Heart2, Heart3 here

    [Header("Video Screens")]
    public VideoPlayer winVideo;   // Drag your 'WinVideoScreen' object here
    public VideoPlayer loseVideo;  // Drag your 'LoseVideoScreen' object here

    [Header("Game Settings")]
    public int maxLives = 3;
    private int currentLives;
    private int currentQuestionIndex = 0;
    private bool isQuizActive = true; // This stops players clicking buttons after Game Over

    [System.Serializable]
    public class Question
    {
        [TextArea] public string questionText;
        public string[] answers;    // Must be exactly 4 answers
        public int correctAnswerID; // 0, 1, 2, or 3
    }
    public Question[] allQuestions;

    void Start()
    {
        // We call this immediately to ensure videos are hidden right when the game starts
        RestartQuiz();
    }

    public void RestartQuiz()
    {
        isQuizActive = true;
        currentLives = maxLives;
        currentQuestionIndex = 0;
        
        // --- THE FIX: Force videos to hide and stop ---
        // This prevents the "ghost image" or video showing up before you play
        if(winVideo != null) 
        { 
            winVideo.Stop(); 
            winVideo.gameObject.SetActive(false); 
        }
        
        if(loseVideo != null) 
        { 
            loseVideo.Stop(); 
            loseVideo.gameObject.SetActive(false); 
        }

        UpdateLivesUI();
        LoadQuestion();
    }

    void LoadQuestion()
    {
        if (currentQuestionIndex < allQuestions.Length)
        {
            // Show the next question
            questionText.text = allQuestions[currentQuestionIndex].questionText;
            for (int i = 0; i < answerButtonTexts.Length; i++)
            {
                answerButtonTexts[i].text = allQuestions[currentQuestionIndex].answers[i];
            }
        }
        else
        {
            // --- WIN CONDITION ---
            Debug.Log("Hacking Complete!");
            isQuizActive = false; // Stop further input so they can't click buttons
            
            if(winVideo != null) 
            {
                winVideo.gameObject.SetActive(true); // Turn on the screen
                winVideo.Play(); // Start the video
            }
        }
    }

    public void SubmitAnswer(int buttonID)
{
    if (!isQuizActive) return;

    // --- SPY CODE STARTS HERE ---
    int correctID = allQuestions[currentQuestionIndex].correctAnswerID;
    Debug.Log("Button Clicked: " + buttonID + " | Correct ID Needed: " + correctID);
    // --- SPY CODE ENDS HERE ---

    if (buttonID == correctID)
    {
        Debug.Log("MATCH! Moving to next question.");
        currentQuestionIndex++;
        LoadQuestion();
    }
    else
    {
        Debug.Log("MISMATCH! You clicked " + buttonID + " but we wanted " + correctID);
        currentLives--;
        UpdateLivesUI();

        if (currentLives <= 0)
        {
            // LOSE CONDITION
            isQuizActive = false;
            if(loseVideo != null) 
            {
                loseVideo.gameObject.SetActive(true);
                loseVideo.Play();
            }
        }
    }
}

    void UpdateLivesUI()
    {
        for (int i = 0; i < livesIcons.Length; i++)
        {
            // Safety check: make sure the icon exists before trying to set it active
            if (livesIcons[i] != null)
            {
                livesIcons[i].SetActive(i < currentLives);
            }
        }
    }
}