using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MiniGameLoader : MonoBehaviour
{
    [Header("Mini-Game Scene Names")]
    public string chessSceneName = "ChessScene";   
    public string racingSceneName = "RacingScene"; 
    // --- ADDED: The name of your new Target game scene ---
    public string targetSceneName = "TargetScene"; 
    // -----------------------------------------------------

    [Header("Main Game References")]
    public GameObject playerCamera; 
    public GameObject computerUI;   
    
    [Tooltip("Drag your Room_Environment folder here to hide the room during games!")]
    public GameObject roomEnvironment; 

    [Tooltip("Drag your Shop_Icon here to hide it while playing mini-games!")]
    public GameObject shopIcon; 

    [Header("Universal Exit Button")]
    [Tooltip("Drag your new Btn_ExitGame here!")]
    public GameObject closeGameButton;

    private bool isMiniGamePlaying = false;
    private string currentActiveMiniGame = "";

    void Start()
    {
        // Automatically hide the close button when the game starts
        if (closeGameButton != null) closeGameButton.SetActive(false);
    }

    // --- ICON BUTTON FUNCTIONS ---

    public void LaunchChessGame()
    {
        if (!isMiniGamePlaying)
        {
            StartCoroutine(LoadMiniGameAsync(chessSceneName));
        }
    }

    public void LaunchTrafficRacer()
    {
        if (!isMiniGamePlaying)
        {
            StartCoroutine(LoadMiniGameAsync(racingSceneName));
        }
    }

    // --- ADDED: The launch function for your new game! ---
    public void LaunchTargetGame()
    {
        if (!isMiniGamePlaying)
        {
            StartCoroutine(LoadMiniGameAsync(targetSceneName));
        }
    }
    // -----------------------------------------------------

    // --- CORE LOADING LOGIC ---

    IEnumerator LoadMiniGameAsync(string sceneName)
    {
        Debug.Log("Launching " + sceneName + "...");

        // 1. Hide the gaming room AND the extra UI!
        if (computerUI != null) computerUI.SetActive(false);
        if (playerCamera != null) playerCamera.SetActive(false);
        if (roomEnvironment != null) roomEnvironment.SetActive(false);
        if (shopIcon != null) shopIcon.SetActive(false); 

        // 2. Show the Universal Exit Button!
        if (closeGameButton != null) closeGameButton.SetActive(true);

        // 3. Load the Mini-Game Scene ON TOP
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        isMiniGamePlaying = true;
        currentActiveMiniGame = sceneName;
        Debug.Log(sceneName + " Running!");
    }

    void Update()
    {
        // We still keep the Escape key working as a backup!
        if (isMiniGamePlaying && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseMiniGame();
        }
    }

    // This is the function your new UI button will call!
    public void CloseMiniGame()
    {
        if (isMiniGamePlaying)
        {
            StartCoroutine(UnloadMiniGameAsync(currentActiveMiniGame));
        }
    }

    IEnumerator UnloadMiniGameAsync(string sceneName)
    {
        Debug.Log("Closing " + sceneName + "...");

        // 1. Hide the Universal Exit Button
        if (closeGameButton != null) closeGameButton.SetActive(false);

        // 2. Unload the game scene
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneName);

        while (!asyncUnload.isDone)
        {
            yield return null;
        }

        // 3. Turn your gaming room environment AND UI back on!
        if (playerCamera != null) playerCamera.SetActive(true);
        if (computerUI != null) computerUI.SetActive(true);
        if (roomEnvironment != null) roomEnvironment.SetActive(true);
        if (shopIcon != null) shopIcon.SetActive(true); 

        isMiniGamePlaying = false;
        currentActiveMiniGame = "";
        Debug.Log("Returned to Desktop!");
    }
}