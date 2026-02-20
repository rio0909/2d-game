using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// --- FIXED: Renamed from GameManager to TargetGameManager to fix the clash! ---
public class TargetGameManager : MonoBehaviour
{
    public Text scoreText;
    public Text goscoreText;
    public GameObject GOPanel;
    public int score;
    // Start is called before the first frame update
    void Start()
    {
        GOPanel.SetActive(false);
        score = 5;
        scoreText.text = "Score: " + score;    
    }

    public void UpdateHitScore()
    {
        score++;
        scoreText.text = "Score: " + score;

    }

    public void UpdateMissScore()
    {
        
        if(score==0 ){
            GameOver();
        }
        else{
            score--;
            scoreText.text = "Score: " + score;
        }
        
    }

    public void GameOver()
    {
        goscoreText.text = "Score: " + score;
        GOPanel.SetActive(true);
    }

    // --- FIXED: Soft restart that doesn't break the main 3D Gaming Room! ---
    public void Restart()
    {
        // 1. Reset the score to default
        score = 5;
        scoreText.text = "Score: " + score;
        
        // 2. Hide the Game Over panel
        GOPanel.SetActive(false);
        
        // 3. Find the Arrow in the scene and reset its position and power
        // (Using FindFirstObjectByType to avoid those yellow Unity warnings!)
        Arrow arrowScript = FindFirstObjectByType<Arrow>();
        if (arrowScript != null)
        {
            arrowScript.reset();
        }
    }


    public IEnumerator QuitGame()
    {
        yield return new WaitForSeconds(3f);
        // save any game data here
        #if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
}