using UnityEngine;

// logic- implementing the Interactable interface i already have
public class NPC_Talk : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Hi Jassi! I see you have the Pro coding setup.");
        // Later will add a real dialogue window 
    }
}