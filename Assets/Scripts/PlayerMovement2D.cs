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

    // --- NEW: Automatically find components to fix the "Unassigned Reference" error ---
    void Awake()
    {
        // If the slots are empty, find the components on this object automatically
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (rb != null)
        {
            rb.gravityScale = 0f; // Ensures he doesn't fall
            rb.freezeRotation = true;
        }
    }

    void Update()
    {
        // 1. Get Input for X (Left/Right) AND Y (Up/Down)
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // 2. Normalize (prevents super-speed diagonal movement)
        if(movement.magnitude > 1) movement = movement.normalized;

        // 3. Animation (Only run if components exist)
        if (animator != null && spriteRenderer != null)
        {
            UpdateAnimation();
        }
    }

    void FixedUpdate()
    {
        // 4. Move Rio
        if (rb != null)
        {
            // Note: In Unity 6 this is 'linearVelocity', in older Unity it is 'velocity'
            rb.linearVelocity = movement * moveSpeed;
        }
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