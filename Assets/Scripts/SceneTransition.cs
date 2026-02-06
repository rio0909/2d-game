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
        if(desktopCanvas != null) desktopCanvas.SetActive(false);
        if(blackScreenPanel != null) blackScreenPanel.color = new Color(0, 0, 0, 0);
    }

    public void EnterComputerMode()
    {
        StartCoroutine(TransitionSequence());
    }

    IEnumerator TransitionSequence()
    {
        float alpha = 0;
        // Fade OUT
        while (alpha < 1)
        {
            alpha += Time.deltaTime * fadeSpeed;
            if (blackScreenPanel != null) blackScreenPanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // Switch
        if (desktopCanvas != null) desktopCanvas.SetActive(true);
        if (player != null) player.SetActive(false);

        // Fade IN
        while (alpha > 0)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            if (blackScreenPanel != null) blackScreenPanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }
}