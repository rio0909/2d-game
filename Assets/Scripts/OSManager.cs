using UnityEngine;
using UnityEngine.Video; 
using System.Collections; 

public class OSManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject startMenuPanel; 
    public GameObject desktopCanvas;  // This should be your DesktopPanel

    [Header("Shutdown FX")]
    public GameObject shutdownScreen; // The Video Screen
    public VideoPlayer shutdownPlayer; 
    public float videoDuration = 3f;   

    [Header("Player Reference")]
    public GameObject player; 

    public void TriggerShutdownSequence()
    {
        StartCoroutine(PlayShutdownAndClose());
    }

    IEnumerator PlayShutdownAndClose()
    {
        Debug.Log("Starting Shutdown...");

        // 1. Hide Start Menu
        if(startMenuPanel != null) startMenuPanel.SetActive(false);

        // 2. Play Video
        if(shutdownScreen != null && shutdownPlayer != null)
        {
            shutdownScreen.SetActive(true);
            shutdownPlayer.Play();
        }

        // 3. Wait for video to finish
        yield return new WaitForSeconds(videoDuration);

        // 4. Run the transition back to the Hall
        CloseComputer();
    }

    void CloseComputer()
    {
        // Hide the Shutdown Screen
        if(shutdownScreen != null) shutdownScreen.SetActive(false);

        // FIND THE TRANSITION SCRIPT
        SceneTransition transition = FindObjectOfType<SceneTransition>();
        
        if (transition != null)
        {
            // This calls the "ReturnSequence" in your other script to fix everything
            transition.ExitComputerMode();
        }
        else
        {
            // EMERGENCY BACKUP: If no transition script is found, force it manually
            if (desktopCanvas != null) desktopCanvas.SetActive(false);
            if (player != null) player.SetActive(true);
            Debug.LogWarning("SceneTransition script not found! Using manual backup.");
        }
    }
}