using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private Transform player;

    void Start()
    {
        if (player == null) return;

        string want = PlayerPrefs.GetString("SpawnId", "");
        if (string.IsNullOrEmpty(want)) return;

        var spawns = Object.FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);
        foreach (var p in spawns)
        {
            if (p.spawnId == want)
            {
                player.position = p.transform.position;
                return;
            }
        }
    }
}
