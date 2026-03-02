using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner Instance { get; private set; }

    [Header("Player")]
    [Tooltip("Optional. If left empty, the spawner will find the Player by tag 'Player' each scene.")]
    [SerializeField] private Transform player;

    [Tooltip("How long to wait after scene load before moving the player.")]
    [SerializeField] private float delayAfterSceneLoad = 0.05f;

    private void Awake()
    {
        // Singleton + destroy duplicates
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Must be root
        if (transform.parent != null)
            transform.SetParent(null);

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // Handles the very first scene when you press Play
        TrySpawnNow();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Spawn after a tiny delay so the Player exists in the new scene
        CancelInvoke(nameof(TrySpawnNow));
        Invoke(nameof(TrySpawnNow), delayAfterSceneLoad);
    }

    private void TrySpawnNow()
    {
        // 1. Find the Player if we don't have them
        if (player == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go != null) player = go.transform;
        }

        if (player == null)
        {
            Debug.LogWarning("PlayerSpawner: Player not found!");
            return;
        }

        // 2. Check for Exact Coordinates
        if (PlayerPrefs.GetInt("UseCoords", 0) == 1)
        {
            float x = PlayerPrefs.GetFloat("SpawnX", 0f);
            float y = PlayerPrefs.GetFloat("SpawnY", 0f);
            
            // Move player to exact coords (keeping original Z just in case)
            player.position = new Vector3(x, y, player.position.z);
            
            // Erase the data so we don't accidentally teleport next time we play
            PlayerPrefs.DeleteKey("UseCoords");
            PlayerPrefs.DeleteKey("SpawnX");
            PlayerPrefs.DeleteKey("SpawnY");
        }
    }
}