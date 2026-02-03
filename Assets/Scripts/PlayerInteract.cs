using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private IInteractable current;

    private void Update()
    {
        // FIX: If the dialogue box is open, STOP! 
        // We return immediately so the 'E' key doesn't trigger a new interaction.
        if (DialogueUI.Instance != null && DialogueUI.Instance.isOpen) return;

        // Normal interaction logic
        if (Input.GetKeyDown(interactKey) && current != null)
        {
            current.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var i = other.GetComponent<IInteractable>();
        if (i != null) current = i;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var i = other.GetComponent<IInteractable>();
        if (i != null && ReferenceEquals(i, current))
            current = null;
    }
}