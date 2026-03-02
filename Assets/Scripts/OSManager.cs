using UnityEngine;
using UnityEngine.Video; 
using System.Collections; 

public class OSManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject startMenuPanel; 
    public GameObject desktopCanvas;  

    [Header("Shutdown FX")]
    public GameObject shutdownScreen; 
    public VideoPlayer shutdownPlayer; 
    public float videoDuration = 3f;   

    [Header("Player Reference")]
    public GameObject player; 

    [Header("Wallpaper Settings")]
    public GameObject defaultScreen;       
    public GameObject stylishWallpaper;    

    [Header("HUD Elements")]
    public GameObject shopIcon; 

    void OnEnable()
    {
        // We only hide the icon if the desktop is actually showing
        UpdateShopIconVisibility();
        CheckAndSetWallpaper();
    }

    // New helper to handle the icon logic
    void UpdateShopIconVisibility()
    {
        if (shopIcon == null) return;

        // If the desktop canvas is active, hide shop. If not, show it.
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
        StartCoroutine(PlayShutdownAndClose());
    }

    IEnumerator PlayShutdownAndClose()
    {
        if(startMenuPanel != null) startMenuPanel.SetActive(false);

        if(shutdownScreen != null && shutdownPlayer != null)
        {
            shutdownScreen.SetActive(true);
            shutdownPlayer.Play();
        }

        yield return new WaitForSeconds(videoDuration);
        CloseComputer();
    }

    void CloseComputer()
    {
        if(shutdownScreen != null) shutdownScreen.SetActive(false);

        SceneTransition transition = FindObjectOfType<SceneTransition>();
        
        if (transition != null)
        {
            transition.ExitComputerMode();
        }
        else
        {
            if (desktopCanvas != null) desktopCanvas.SetActive(false);
            if (player != null) player.SetActive(true);
        }

        // Force the shop icon back on when we close
        if (shopIcon != null) shopIcon.SetActive(true);

        BadgeManager bm = FindObjectOfType<BadgeManager>();
        if (bm != null)
        {
            bm.ShowPendingBadges();
        }
    }
}