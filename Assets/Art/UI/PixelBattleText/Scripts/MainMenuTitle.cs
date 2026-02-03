using UnityEngine;
using System.Collections;
using PixelBattleText; 

public class MainMenuTitle : MonoBehaviour
{
    public TextAnimation titleAnimation; 
    public string gameTitle = "StudyVerse";
    
    [Header("Loop Settings")]
    public float loopDelay = 4.0f; 
    // I removed the [cite] tag from the comment below too
    public Vector2 screenPosition = new Vector2(0.5f, 0.7f); 

    void Start()
    {
        StartCoroutine(SpawnTitleLoop());
    }

    IEnumerator SpawnTitleLoop()
    {
        while (true)
        {
            // DELETE "" FROM HERE
            // 1. Spawn the text at the specific screen position
            PixelBattleTextController.DisplayText(gameTitle, titleAnimation, screenPosition);

            // 2. Wait for the animation to finish before spawning the next one
            yield return new WaitForSeconds(loopDelay);
        }
    }
}