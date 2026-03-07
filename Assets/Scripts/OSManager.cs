using UnityEngine;
using UnityEngine.Video; 
using System.Collections; 

public class OSManager : MonoBehaviour
{
    // dragging in all the ui stuff 
    [Header("UI Elements")]
    public GameObject startMenuPanel; 
    public GameObject desktopCanvas;  

    // fancy shutdown video stuff
    [Header("Shutdown FX")]
    public GameObject shutdownScreen; 
    public VideoPlayer shutdownPlayer; 
    public float videoDuration = 3f;   

    // where did the player go waaaa
    [Header("Player Reference")]
    public GameObject player; 

    // backgrounds
    [Header("Wallpaper Settings")]
    public GameObject defaultScreen;       
    public GameObject stylishWallpaper;    

    // hud stuff
    [Header("HUD Elements")]
    public GameObject shopIcon; 

    void OnEnable()
    {
        // only hide the icon if the desktop is actually on screen so it doesn't vanish forever
        UpdateShopIconVisibility();
        CheckAndSetWallpaper();
    }

    // helper to deal with the shop icon so it stops breaking mumu
    void UpdateShopIconVisibility()
    {
        if (shopIcon == null) return; // safety first 

        // if they are looking at the desktop hide the shop, otherwise put it back
        if (desktopCanvas != null && desktopCanvas.activeInHierarchy)
        {
            shopIcon.SetActive(false);
        }
        else
        {
            shopIcon.SetActive(true);
        }
    }

    public void CheckAndSetWallpaper()
    {
        // gotta check what wallpaper they actually bought 
        int wallpaperID = PlayerPrefs.GetInt("WallpaperID", 0);

        if (wallpaperID == 1)
        {
            if (defaultScreen != null) defaultScreen.SetActive(false);
            if (stylishWallpaper != null) stylishWallpaper.SetActive(true);
        }
        else
        {
            if (defaultScreen != null) defaultScreen.SetActive(true);
            if (stylishWallpaper != null) stylishWallpaper.SetActive(false);
        }
    }

    public void TriggerShutdownSequence()
    {
        // time to go to sleep mumu
        StartCoroutine(PlayShutdownAndClose());
    }

    IEnumerator PlayShutdownAndClose()
    {
        if(startMenuPanel != null) startMenuPanel.SetActive(false);

        // play the little video if i actually remembered to link the player in the inspector
        if(shutdownScreen != null && shutdownPlayer != null)
        {
            shutdownScreen.SetActive(true);
            shutdownPlayer.Play();
        }

        // just stare at the screen for a few seconds waaaa
        yield return new WaitForSeconds(videoDuration);
        CloseComputer();
    }

    void CloseComputer()
    {
        if(shutdownScreen != null) shutdownScreen.SetActive(false);

        // try to use the fancy scene transition
        SceneTransition transition = FindObjectOfType<SceneTransition>();
        
        if (transition != null)
        {
            transition.ExitComputerMode();
        }
        else
        {
            // fallback just in case the transition manager is missing again 
            if (desktopCanvas != null) desktopCanvas.SetActive(false);
            if (player != null) player.SetActive(true);
        }

        // force the shop icon back on when we close or they can't buy anything waaaa
        if (shopIcon != null) shopIcon.SetActive(true);

        // pop any badges they earned while staring at the pc
        BadgeManager bm = FindObjectOfType<BadgeManager>();
        if (bm != null)
        {
            bm.ShowPendingBadges();
        }
    }
}