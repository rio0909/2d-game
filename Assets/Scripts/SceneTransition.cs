using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// The name below MUST match your filename "SceneTransition"
public class SceneTransition : MonoBehaviour
{
    [Header("UI Elements")]
    public Image blackScreenPanel;  
    public GameObject desktopCanvas; 
    public GameObject player;       

    [Header("Settings")]
    public float fadeSpeed = 2f;

    [Header("Cursor Settings")]
    public Texture2D customCursor; 

    void Start()
    {
        if (customCursor != null) Cursor.SetCursor(customCursor, Vector2.zero, CursorMode.ForceSoftware);
        if (desktopCanvas != null) desktopCanvas.SetActive(false);
        // Ensure black screen is transparent at start
        if (blackScreenPanel != null) blackScreenPanel.color = new Color(0, 0, 0, 0);
    }

    // Call this when sitting down at the PC
    public void EnterComputerMode()
    {
        StopAllCoroutines();
        StartCoroutine(TransitionToPC());
    }

    // Call this from OSManager when shutting down
    public void ExitComputerMode()
    {
        StopAllCoroutines();
        StartCoroutine(TransitionToHall());
    }

    IEnumerator TransitionToPC()
    {
        float alpha = 0;
        // 1. Fade to Black
        while (alpha < 1)
        {
            alpha += Time.deltaTime * fadeSpeed;
            if (blackScreenPanel != null) blackScreenPanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // 2. Switch Views
        if (desktopCanvas != null) desktopCanvas.SetActive(true);
        if (player != null) player.SetActive(false); // Hides player in PC mode

        // 3. Fade Out Black
        while (alpha > 0)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            if (blackScreenPanel != null) blackScreenPanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }

    IEnumerator TransitionToHall()
    {
        float alpha = 0;
        // 1. Fade to Black (from PC screen)
        while (alpha < 1)
        {
            alpha += Time.deltaTime * fadeSpeed;
            if (blackScreenPanel != null) blackScreenPanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // 2. Switch Views back to Lecture Hall
        if (desktopCanvas != null) desktopCanvas.SetActive(false);
        if (player != null) player.SetActive(true); // REVEALS player in Hall

        // 3. Fade Out Black so we can see the room
        while (alpha > 0)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            if (blackScreenPanel != null) blackScreenPanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }
}