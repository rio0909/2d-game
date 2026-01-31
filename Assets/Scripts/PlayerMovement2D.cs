using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 5f;

    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    private float horizontalInput;

    void Start()
    {
        // Keep gravity so he sits on the floor
        rb.gravityScale = 2f; 
        rb.freezeRotation = true; 
    }

    void Update()
    {
        // 1. Get Input (Left/Right only)
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // 2. Animate
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        // 3. Move Physics
        // We keep 'rb.velocity.y' so gravity still works (he falls if he walks off a cliff)
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    void UpdateAnimation()
    {
        // Send Speed to Animator
        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));

        // Flip Sprite
        if (horizontalInput > 0) spriteRenderer.flipX = false;
        else if (horizontalInput < 0) spriteRenderer.flipX = true;
    }
}