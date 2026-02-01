using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 5f;

    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    private Vector2 movement;

    void Start()
    {
        rb.gravityScale = 0f; // Ensures he doesn't fall
        rb.freezeRotation = true;
    }

    void Update()
    {
        // 1. Get Input for X (Left/Right) AND Y (Up/Down)
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical"); // This was missing before!

        // 2. Normalize (prevents super-speed diagonal movement)
        movement = movement.normalized;

        // 3. Animation
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        // 4. Move Rio
        rb.linearVelocity = movement * moveSpeed;
    }

    void UpdateAnimation()
    {
        // Animate if moving in ANY direction
        animator.SetFloat("Speed", movement.sqrMagnitude);

        // Flip Sprite (Only flip for Left/Right)
        if (movement.x > 0) spriteRenderer.flipX = false;
        else if (movement.x < 0) spriteRenderer.flipX = true;
    }
}