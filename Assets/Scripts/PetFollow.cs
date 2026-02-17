using UnityEngine;

public class PetFollow : MonoBehaviour
{
    [Header("Script Setup")]
    public Transform player;        

    [Header("Movement Settings")]
    public float followSpeed = 3.5f; // Locked to your setting
    
    [Header("Position Tuning")]
    public Vector3 offset = new Vector3(-0.9f, 0.8f, 0f); // Locked to your setting

    [Header("Size Tuning")]
    public float normalSize = 1f; // Locked to your setting

    void Awake()
    {
        // This makes the pet stay alive when you change rooms
        DontDestroyOnLoad(this.gameObject);
    }

    void LateUpdate()
    {
        // If we enter a new room, the 'player' slot will become empty.
        // This code automatically searches for the player again.
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null) 
            {
                player = foundPlayer.transform;
            }
            else return; // Wait until the player is found
        }

        // Apply the exact movement and offset you like
        Vector3 targetPosition = player.position + offset;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, followSpeed * Time.deltaTime);

        HandleSizeAndFlip();
    }

    void HandleSizeAndFlip()
    {
        float direction = (player.position.x > transform.position.x) ? 1 : -1;
        transform.localScale = new Vector3(direction * normalSize, normalSize, 1);
    }
}