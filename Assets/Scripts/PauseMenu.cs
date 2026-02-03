using UnityEngine;
using UnityEngine.SceneManagement; // Needed to switch scenes

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI; // Link your Panel here

    void Update()
    {
        // Listen for the ESC key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false); // Hide the menu
        Time.timeScale = 1f;          // UNFREEZE time
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);  // Show the menu
        Time.timeScale = 0f;          // FREEZE time
        GameIsPaused = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f; // Must unfreeze before leaving, or the menu will be stuck!
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}