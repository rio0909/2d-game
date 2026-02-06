using UnityEngine;
using UnityEngine.Events;

public class ComputerInteraction : MonoBehaviour
{
    [Header("Settings")]
    public KeyCode interactKey = KeyCode.E; // The key to press
    public GameObject pressEPrompt; // Optional: Drag a "Press E" text UI here later

    [Header("Events")]
    public UnityEvent onInteract; // Link this to your SceneTransition

    private bool playerInRange = false; // Is the player standing here?

    // 1. Detect when player walks INTO the box collider
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Player is near computer. Press E!");
            
            // If you have a prompt, show it
            if (pressEPrompt != null) pressEPrompt.SetActive(true);
        }
    }

    // 2. Detect when player walks OUT
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            
            // Hide the prompt
            if (pressEPrompt != null) pressEPrompt.SetActive(false);
        }
    }

    // 3. Listen for the Key Press every frame
    void Update()
    {
        // If player is close AND presses 'E'
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            Debug.Log("Interacting...");
            // Hide prompt immediately so it doesn't float over the loading screen
            if (pressEPrompt != null) pressEPrompt.SetActive(false);
            
            // Trigger the transition!
            onInteract.Invoke();
        }
    }
}