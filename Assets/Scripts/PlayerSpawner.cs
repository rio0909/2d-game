using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner Instance { get; private set; }

    [Header("Player")]
    [Tooltip("Optional. If left empty, the spawner will find the Player by tag 'Player' each scene.")]
    [SerializeField] private Transform player;

    [Header("Spawn Settings")]
    [Tooltip("PlayerPrefs key used by ScenePortal to store the target spawn id.")]
    [SerializeField] private string spawnPrefKey = "SpawnId";

    [Tooltip("How long to wait after scene load before searching spawn points (helps when objects spawn on Start).")]
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
        // Spawn after a tiny delay so SpawnPoints/Player exist
        CancelInvoke(nameof(TrySpawnNow));
        Invoke(nameof(TrySpawnNow), delayAfterSceneLoad);
    }

    private void TrySpawnNow()
    {
        // If player not assigned or got destroyed, try find by tag
        if (player == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go != null) player = go.transform;
        }

        if (player == null)
        {
            Debug.LogWarning("PlayerSpawner: Player not found. Make sure your player has tag 'Player' or assign it in Inspector.");
            return;
        }

        string want = PlayerPrefs.GetString(spawnPrefKey, "");
        if (string.IsNullOrEmpty(want))
            return; // no requested spawn

        var spawns = Object.FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);
        foreach (var p in spawns)
        {
            if (p.spawnId == want)
            {
                player.position = p.transform.position;

                // Optional: clear after using so it doesn't keep re-teleporting on future loads
                PlayerPrefs.DeleteKey(spawnPrefKey);

                return;
            }
        }

        Debug.LogWarning($"PlayerSpawner: No SpawnPoint found with spawnId '{want}' in this scene.");
    }
}
