using Google.Protobuf.WellKnownTypes;
using System.Runtime.CompilerServices;
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
    private float groundVelocityX = 0f;

    [Header("Jumping")]
    public float jumpForce = 15f;
    private bool isJumpCancelled = false;
    public float baseGravity = 2.5f;
    public float baseFallGravity = 7f;
    public float shortJumpGravity = 7f;
    public float maxFallSpeed = 10f;
    private float coyoteTime = 0f;
    public float maxCoyoteTime = 0.1f;

    [Header("StandableCheck")]
    public Transform standablePosCheck;
    public Vector2 standableCheckSize = new Vector2(0.5f, 0.1f);
    public LayerMask standableMask;

    [Header("ML")]
    private bool agentJumpHeld = false;
    public bool IsGroundedPublic { get; private set; }
    public float CoyoteTime01
    {
        get { return Mathf.Clamp01(coyoteTime / maxCoyoteTime); }
    }

    private void Update()
    { 
        if (IsGroundedPublic)
        {
            coyoteTime = maxCoyoteTime;
        }
        else
        {
            coyoteTime -= Time.deltaTime;
        }

        float animVelocityY = rb.linearVelocityY;

        //to get rid of an animation bug
        if (IsGroundedPublic && Mathf.Abs(animVelocityY) < 2f)
            animVelocityY = 0f;

        animator.SetFloat("yVelocity", animVelocityY);
        animator.SetFloat("magnitude", Mathf.Abs(horizontalMovement));

        if (horizontalMovement < 0f)
        {
            FlipLeft();
        }
        else if (horizontalMovement > 0f)
        {
            FlipRight();
        }
    }


    private void FixedUpdate()
    {
        IsGroundedPublic = IsOnStandable();
        rb.linearVelocityX = CalculateDesiredVelocityX();
        Gravity();

    }

    private float CalculateDesiredVelocityX()
    {
        float platformVelocityX = IsGroundedPublic ? groundVelocityX : 0f;
        float desiredWorldX = platformVelocityX + horizontalMovement * moveSpeed;

        return Mathf.Clamp(desiredWorldX, -moveSpeed * 1.2f, moveSpeed * 1.2f);
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
        if (rb.linearVelocityY > 0f)
        {
            rb.gravityScale = isJumpCancelled ? shortJumpGravity : baseGravity;
        }
        else if (rb.linearVelocityY < 0f)
        {
            rb.gravityScale = baseFallGravity;
            rb.linearVelocityY = Mathf.Max(rb.linearVelocityY, -maxFallSpeed);
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    //also calculates ground velocity when on moving platforms
    private bool IsOnStandable()
    {
        Collider2D hit = Physics2D.OverlapBox(standablePosCheck.position, standableCheckSize, 0f, standableMask);
        if(hit == null)
        {
            groundVelocityX = 0f;
            return false;
        }

        Rigidbody2D groundRb = hit.attachedRigidbody;

        if(groundRb != null)
        {
            groundVelocityX = groundRb.linearVelocityX;
        }
        else
        {
            groundVelocityX = 0f;
        }

        return true;
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
        Gizmos.DrawWireCube(standablePosCheck.position, standableCheckSize);
    }

    //ML agents
    public void SetMove(float x)
    {
        horizontalMovement = Mathf.Clamp(x, -1f, 1f);
    }

    private void JumpPressed()
    {
        if (CanJump())
        {
            isJumpCancelled = false;
            rb.linearVelocityY = jumpForce;
            coyoteTime = 0f;
            if (animator) animator.SetTrigger("jump");
        }
    }

    public void SetJumpHeld(bool held)
    {
        if (held && !agentJumpHeld)
        {
            JumpPressed();
        }

        if (!held && agentJumpHeld && rb.linearVelocityY > 0f)
        {
            isJumpCancelled = true;
        }

        agentJumpHeld = held;
    }


}
