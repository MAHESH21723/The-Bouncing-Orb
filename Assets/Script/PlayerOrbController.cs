using UnityEngine;

// Rolls the orb left/right and makes it jump.
// Keyboard: A/D or arrows to roll, Space / W / Up to jump. On phones the on-screen buttons set TouchInput.
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerOrbController : MonoBehaviour
{
    [Header("Movement")]
    public float moveAcceleration = 10f;
    public float maxHorizontalSpeed = 5f;

    [Header("Jump")]
    public float jumpForce = 6f;
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    // Set by the on-screen touch buttons.
    public static float TouchHorizontal;
    public static bool TouchJumpPressed;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float jumpBufferTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!CanPlay()) return;
        HandleGroundCheck();
        HandleJumpInput();
    }

    private void FixedUpdate()
    {
        if (!CanPlay()) return;
        HandleHorizontalMovement();
    }

    private static bool CanPlay()
    {
        return GameManager.Instance == null || GameManager.Instance.IsPlaying;
    }

    private void HandleHorizontalMovement()
    {
        float inputX = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
        if (Mathf.Abs(TouchHorizontal) > 0.01f) inputX = TouchHorizontal;

        // Apply horizontal acceleration
        rb.AddForce(new Vector2(inputX * moveAcceleration, 0f), ForceMode2D.Force);

        // Clamp max horizontal speed
        Vector2 vel = rb.linearVelocity;
        vel.x = Mathf.Clamp(vel.x, -maxHorizontalSpeed, maxHorizontalSpeed);
        rb.linearVelocity = vel;
    }

    private void HandleGroundCheck()
    {
        // Simple circle overlap just under the orb to detect ground.
        // Measured straight down from the centre (not from the child point), because the orb rolls and rotates.
        float offset = groundCheckPoint != null ? Vector2.Distance(groundCheckPoint.position, transform.position) : 0.55f;
        Vector2 checkPos = (Vector2)transform.position + Vector2.down * offset;
        isGrounded = Physics2D.OverlapCircle(checkPos, Mathf.Max(groundCheckRadius, 0.15f), groundLayer);
    }

    private void HandleJumpInput()
    {
        bool pressed = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || TouchJumpPressed;
        TouchJumpPressed = false;

        // Remember a jump pressed just before landing, so it still happens.
        if (pressed) jumpBufferTimer = 0.15f;
        else jumpBufferTimer -= Time.deltaTime;

        if (jumpBufferTimer > 0f && isGrounded)
        {
            jumpBufferTimer = 0f;
            // Clear existing vertical velocity then apply jump
            Vector2 vel = rb.linearVelocity;
            vel.y = 0f;
            rb.linearVelocity = vel;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    public void ResetAt(Vector2 position)
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.position = position;
        transform.position = position;
        transform.rotation = Quaternion.identity;
        var trail = GetComponent<TrailRenderer>();
        if (trail != null) trail.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
    }
}
