using UnityEngine;

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

    private Rigidbody2D rb;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        HandleGroundCheck();
        HandleJumpInput();
    }

    private void FixedUpdate()
    {
        HandleHorizontalMovement();
    }
    private void HandleHorizontalMovement()
    {
        float inputX = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
        // Apply horizontal acceleration
        rb.AddForce(new Vector2(inputX * moveAcceleration, 0f), ForceMode2D.Force);

        // Clamp max horizontal speed
        Vector2 vel = rb.linearVelocity;
        vel.x = Mathf.Clamp(vel.x, -maxHorizontalSpeed, maxHorizontalSpeed);
        rb.linearVelocity = vel;
    }

    private void HandleGroundCheck()
    {
        // Simple circle overlap under the orb to detect ground
        if (groundCheckPoint != null)
        {
            isGrounded = Physics2D.OverlapCircle(
                groundCheckPoint.position,
                groundCheckRadius,
                groundLayer
            );
        }
        else
        {
            isGrounded = false;
        }
    }
    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // Clear existing vertical velocity then apply jump
            Vector2 vel = rb.linearVelocity;
            vel.y = 0f;
            rb.linearVelocity = vel;

            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
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
