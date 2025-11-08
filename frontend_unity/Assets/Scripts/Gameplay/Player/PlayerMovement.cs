using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;

    [Header("Movement")]
    public float moveSpeed = 6f;
    private float horizontalMovement;

    [Header("Jumping")]
    public float jumpForce = 15f;
    private bool isJumpCancelled = false;
    public float baseGravity = 2.5f;
    public float baseFallGravity = 7f;
    public float shortJumpGravity = 7f;
    public float maxFallSpeed = 10f;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
    public LayerMask groundLayer;


    private void FixedUpdate()
    {
        rb.linearVelocityX = horizontalMovement * moveSpeed;
        Gravity();

    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            isJumpCancelled = false;
            rb.linearVelocityY = jumpForce;    
        }
        else if (context.canceled)
        {
            isJumpCancelled = true;
        }
    }

    private void Gravity()
    {
        if(rb.linearVelocityY < 0 && !isJumpCancelled)
        {
            rb.gravityScale = baseFallGravity;
            rb.linearVelocityY = Mathf.Max(rb.linearVelocityY, -maxFallSpeed);
        }
        else if(isJumpCancelled)
        {
            rb.gravityScale = shortJumpGravity;
            rb.linearVelocityY = Mathf.Max(rb.linearVelocityY, -maxFallSpeed);
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    private bool IsGrounded()
    {
        if(Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0f, groundLayer))
        {
            return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }


}
