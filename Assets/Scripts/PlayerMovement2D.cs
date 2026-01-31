using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement2D : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 4.5f;

    [Header("Components")]
    [SerializeField] private Animator animator; // Assign this in Inspector

    private Rigidbody2D rb;
    private Vector2 input;
    private Vector2 lastMoveDirection; // Stores the last non-zero direction

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Ensure physics settings are correct
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        // Collision detection: Continuous is better for preventing wall clipping
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; 
    }

    private void Update()
    {
        // 1. Get Input
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        // 2. Normalize to prevent faster diagonal movement
        if (input.sqrMagnitude > 1f) input.Normalize();

        // 3. Update Animation Logic
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        // 4. Move Rigidbody
        rb.MovePosition(rb.position + input * moveSpeed * Time.fixedDeltaTime);
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;

        // If currently moving, update the "last known direction"
        if (input.sqrMagnitude > 0.01f)
        {
            lastMoveDirection = input;
        }

        // Send parameters to the Animator
        // Use 'lastMoveDirection' for x/y so we don't snap to (0,0) when stopping
        animator.SetFloat("Horizontal", lastMoveDirection.x);
        animator.SetFloat("Vertical", lastMoveDirection.y);
        
        // Use 'input' magnitude for speed to know IF we are walking or idle
        animator.SetFloat("Speed", input.sqrMagnitude);
    }
}