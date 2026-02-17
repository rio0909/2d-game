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

    // --- NEW SECTION: WALLPAPER SETTINGS ---
    [Header("Wallpaper Settings")]
    public GameObject defaultScreen;       // Drag 'Desktop_PanelScreen' here
    public GameObject stylishWallpaper;    // Drag 'Stylish_Wallpaper' here

    // This function runs AUTOMATICALLY every time you open the computer
    void OnEnable()
    {
        CheckAndSetWallpaper();
    }

    public void CheckAndSetWallpaper()
    {
        // 1. Read the Save File (0 = Default, 1 = Stylish)
        int wallpaperID = PlayerPrefs.GetInt("WallpaperID", 0);

        // 2. Toggle the correct objects
        if (wallpaperID == 1)
        {
            // User bought the Stylish one!
            if (defaultScreen != null) defaultScreen.SetActive(false);
            if (stylishWallpaper != null) stylishWallpaper.SetActive(true);
        }
        else
        {
            // User has default (or hasn't bought anything yet)
            if (defaultScreen != null) defaultScreen.SetActive(true);
            if (stylishWallpaper != null) stylishWallpaper.SetActive(false);
        }
    }
    // ---------------------------------------

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