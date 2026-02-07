using UnityEngine;
using UnityEngine.Video; 
using System.Collections; 

public class OSManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject startMenuPanel; 
    public GameObject desktopCanvas;  // The Computer Screen

    [Header("Shutdown FX")]
    public GameObject shutdownScreen; // The Black Screen / Video
    public VideoPlayer shutdownPlayer; 
    public float videoDuration = 3f;   

    [Header("Player Reference")]
    public GameObject player; // We need this to turn him back on!

    void Start()
    {
        // --- MAGIC FIX: Auto-find the player if the slot is empty ---
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

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

        // 3. Wait
        yield return new WaitForSeconds(videoDuration);

        // 4. Close Everything
        CloseComputer();
    }

    void CloseComputer()
    {
        // Hide the Shutdown Screen
        if(shutdownScreen != null) shutdownScreen.SetActive(false);

        // Hide the Computer UI
        desktopCanvas.SetActive(false);
        
        // --- CRITICAL FIX: Turn Rio back ON ---
        if (player != null)
        {
            player.SetActive(true);
        }
        else
        {
            // If we still can't find him, try one last time
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if(foundPlayer != null) foundPlayer.SetActive(true);
        }
    }
}