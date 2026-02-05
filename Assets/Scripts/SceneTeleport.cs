using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTeleport : MonoBehaviour
{
    [Header("Settings")]
    public string sceneToLoad = "Campus";

    private bool playerIsAtDoor = false; // To track if we are close enough

    void Update()
    {
        // We only check for input if the player is actually standing at the door
        if (playerIsAtDoor && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Teleporting to: " + sceneToLoad);
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    // When Rio walks INTO the door zone
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsAtDoor = true;
            // Optional: You could show a "Press E" UI prompt here!
            Debug.Log("Player at door. Waiting for input...");
        }
    }

    // When Rio walks AWAY from the door
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsAtDoor = false;
        }
    }
}