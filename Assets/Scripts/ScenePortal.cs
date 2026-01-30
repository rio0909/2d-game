using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePortal : MonoBehaviour, IInteractable
{
    [SerializeField] private string targetSceneName = "LectureHall";
    [SerializeField] private string targetSpawnId = "LectureHall";

    public void Interact()
    {
        // remember where to spawn in next scene
        PlayerPrefs.SetString("SpawnId", targetSpawnId);
        PlayerPrefs.Save();

        // load scene
        SceneManager.LoadScene(targetSceneName);
    }
}
