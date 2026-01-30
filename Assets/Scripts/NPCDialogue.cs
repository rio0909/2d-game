using UnityEngine;

public class NPCDialogue : MonoBehaviour, IInteractable
{
    [SerializeField] private string npcName = "ajay";
    [TextArea(2, 5)]
    [SerializeField] private string[] lines;

    public void Interact()
    {
        DialogueUI.Instance.Show(npcName, lines);
    }
}
