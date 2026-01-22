using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class SlimeController : MonoBehaviour, IStompable, IKillsPlayer
{
    public float speed = 1f;
    public Transform groundCheck;
    public Transform obstacleCheck;
    public LayerMask groundMask;
    public LayerMask obstacleMask;
    public float groundCheckDistance = 0.2f;
    public float obstacleCheckDistance = 0.1f;

    public Rigidbody2D rb;
    private bool movingLeft = true;

    public Animator animator;


    void FixedUpdate()
    {
        rb.linearVelocityX = movingLeft ? -speed : speed;
        if (!IsGroundAhead())
        {
            Flip();
        }

        if (IsObstacleHit()) { 
            Flip();
        }
    }

    public void OnStomped(PlayerController player, Collision2D collision)
    {
        if (collision.contacts[0].normal.y > 0.5f)
        {
            StopCollidingAndMoving();
            animator.SetTrigger("Die");
        }
        else
        {
            KillPlayer(player);
        }
    }

    private void StopCollidingAndMoving()
    {
        foreach (var c in GetComponentsInChildren<Collider2D>())
        {
            c.enabled = false;
        }
        rb.simulated = false;
        rb.linearVelocity = Vector2.zero;
    }

    public void KillPlayer(PlayerController player)
    {
        player.Die();
    }

    public void OnDeathAnimationEnd()
    {
        Destroy(gameObject);
    }

    private bool IsGroundAhead()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundMask);
    }

    private bool IsObstacleHit()
    {
        Vector2 direction = movingLeft ? Vector2.left : Vector2.right;
        return Physics2D.Raycast(obstacleCheck.position, direction, obstacleCheckDistance, obstacleMask);
    }

    private void Flip()
    {
        movingLeft = !movingLeft;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);

        Gizmos.color = Color.red;
        Vector3 direction = movingLeft ? Vector3.left : Vector3.right;
        Gizmos.DrawLine(obstacleCheck.position, obstacleCheck.position + direction * obstacleCheckDistance);
    }
}
