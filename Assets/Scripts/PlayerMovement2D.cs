using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement2D : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 4.5f;

    [Header("Components")]
    [SerializeField] private Animator animator; 
    [SerializeField] private SpriteRenderer spriteRenderer; 

    private Rigidbody2D rb;
    private Vector2 input;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; 

        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // 1. Get Input
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        // 2. Normalize
        if (input.sqrMagnitude > 1f) input.Normalize();

        // 3. Handle Animation & Flipping
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        // 4. Move
        rb.MovePosition(rb.position + input * moveSpeed * Time.fixedDeltaTime);
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;

        // FIXED: Only sending "Speed" because that is the only parameter we made.
        // We removed "Horizontal" and "Vertical" to stop the errors.
        animator.SetFloat("Speed", input.sqrMagnitude);

        // Flipping Logic
        if (input.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (input.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }
}