using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePortal : MonoBehaviour, IInteractable
{
    [SerializeField] private string targetSceneName = "LectureHall";
    
    [Header("Exact Spawn Coordinates")]
    [Tooltip("Type the exact X and Y coordinates the player should land on in the new scene.")]
    [SerializeField] private Vector2 targetPosition;

    public void Interact()
    {
        // 1. Save the exact coordinates into PlayerPrefs
        PlayerPrefs.SetFloat("SpawnX", targetPosition.x);
        PlayerPrefs.SetFloat("SpawnY", targetPosition.y);
        PlayerPrefs.SetInt("UseCoords", 1); // A flag to tell the spawner to use coords
        PlayerPrefs.Save();

        // 2. Load the scene
        SceneManager.LoadScene(targetSceneName);
    }
}