using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private Transform player;

    void Start()
    {
        if (player == null) return;

        string want = PlayerPrefs.GetString("SpawnId", "");
        if (string.IsNullOrEmpty(want)) return;

        var points = FindObjectsOfType<SpawnPoint>();
        foreach (var p in points)
        {
            if (p.spawnId == want)
            {
                player.position = p.transform.position;
                return;
            }
        }
    }
}
