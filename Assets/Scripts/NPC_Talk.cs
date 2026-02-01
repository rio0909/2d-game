using UnityEngine;

// logic: We implement the "IInteractable" interface you already have!
public class NPC_Talk : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Hi Rio! I see you have the Pro coding setup.");
        // Later, we will add a real dialogue window here!
    }
}