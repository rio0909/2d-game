using UnityEngine;
using UnityEngine.SceneManagement; // Required to change scenes!

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
{
    // Load the specific scene by its exact name
    SceneManager.LoadScene("Campus");
}

    public void QuitGame()
    {
        Debug.Log("The game has quit!"); // Just for testing in Editor
        Application.Quit();
    }
}