using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    public Animator animator;

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
    private float coyoteTime = 0f; //time after leaving ground where jump is still possible
    public float maxCoyoteTime = 0.1f;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
    public LayerMask groundLayer;

    private void Update()
    {
        if (IsGrounded())
        {
            coyoteTime = maxCoyoteTime;
        }
        else
        {
            coyoteTime -= Time.deltaTime;
        }

        animator.SetFloat("yVelocity", rb.linearVelocityY);
        animator.SetFloat("magnitude", rb.linearVelocity.magnitude);

        if (rb.linearVelocityX < 0f)
        {
            FlipLeft();
        }
        else if (rb.linearVelocityX > 0f)
        {
            FlipRight();
        }
    }


    private void FixedUpdate()
    {
        rb.linearVelocityX = horizontalMovement * moveSpeed;
        Gravity();

    }

    //TODO get rid of old input system in other files
    //TODO spamming jump makes character appear moving back and forth
    //TODO change character box collider
    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        //succesful jump initiation
        if (context.performed && CanJump())
        {
            isJumpCancelled = false;
            rb.linearVelocityY = jumpForce;
            coyoteTime = 0f; //prevent double jumps
            animator.SetTrigger("jump");
        }

        //jump cancelled mid air
        else if (context.canceled)
        {
            isJumpCancelled = true;
            if(rb.linearVelocityY > 0f)
            {
                animator.SetTrigger("jump");
            }
        }
    }

    private void Gravity()
    {
        //normal jump
        if(rb.linearVelocityY < 0f && !isJumpCancelled)
        {
            rb.gravityScale = baseFallGravity;
            rb.linearVelocityY = Mathf.Max(rb.linearVelocityY, -maxFallSpeed);
        }
        //short jump
        else if (isJumpCancelled)
        {
            rb.gravityScale = shortJumpGravity;
            rb.linearVelocityY = Mathf.Max(rb.linearVelocityY, -maxFallSpeed);
        }
        //default gravity- walking, standing,  falling etc.
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0f, groundLayer);
    }

    private bool CanJump()
    {
        return coyoteTime > 0f;

    }

    private void FlipLeft()
    {
        Vector3 scale = transform.localScale;
        scale.x = -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    private void FlipRight()
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }


}
