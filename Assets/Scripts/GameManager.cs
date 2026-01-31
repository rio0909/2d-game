using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Debug")]
    [SerializeField] private bool logLifecycle = false;

    private void Awake()
    {
        // Destroy duplicates
        if (Instance != null && Instance != this)
        {
            if (logLifecycle) Debug.Log("[GameManager] Duplicate detected, destroying this one.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Must be root object; if not, unparent
        if (transform.parent != null)
            transform.SetParent(null);

        DontDestroyOnLoad(gameObject);

        if (logLifecycle) Debug.Log("[GameManager] Now persistent (DontDestroyOnLoad).");
    }
}
