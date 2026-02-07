using UnityEngine;

// I changed the class name to ComputerSystem to match the new file
public class ComputerSystem : MonoBehaviour
{
    [Header("App Windows")]
    public GameObject cyberQuizWindow;

    public void OpenCyberQuiz()
    {
        if(cyberQuizWindow != null) 
            cyberQuizWindow.SetActive(true);
    }

    public void CloseCyberQuiz()
    {
        if(cyberQuizWindow != null) 
            cyberQuizWindow.SetActive(false);
    }
}