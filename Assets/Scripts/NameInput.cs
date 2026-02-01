using UnityEngine;
using TMPro; // Needed for the text box

public class NameInput : MonoBehaviour
{
    public TMP_InputField inputField;
    public GameObject panelToHide;

    public void SaveName()
    {
        if (inputField.text.Length > 0)
        {
            // 1. Save the name to our static memory
            GameData.playerName = inputField.text;

            // 2. Hide the input box so the game can start
            panelToHide.SetActive(false);
            
            Debug.Log("Welcome, Student: " + GameData.playerName);
        }
    }
}