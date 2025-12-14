using UnityEngine;

public class SlimeController : MonoBehaviour, IStompable, IDamageDealer
{
    public float speed = 1f;
    public Transform groundCheck;
    public Transform wallCheck;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.2f;
    public float wallCheckDistance = 0.1f;

    public Rigidbody2D rb;
    private bool movingLeft = true;

    public Animator animator;

    void OnEnable()
    {
        if (Mode.IsEditorMode)
        {
            Debug.Log("In editor mode- slime");
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        else
        {
            Debug.Log("In play mode- slime");
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }


    void FixedUpdate()
    {
        if (Mode.IsEditorMode)
        {
            return;
        }

        rb.linearVelocityX = movingLeft ? -speed : speed;
        if (!IsGroundAhead())
        {
            Flip();
        }

        if (IsWallHit()) { 
            Flip();
        }
    }

    public void OnStomped(PlayerController player, Collision2D collision)
    {
        if (collision.contacts[0].normal.y > 0.5f)
        {
            Debug.Log("Slime stomped!");
            Die();
        }
        else
        {
            Debug.Log("Player hit slime from the side, taking damage");
            DealDamage(player);
        }
    }

    public void DealDamage(PlayerController player)
    {
        Debug.Log("Slime dealing damage to player");
        player.TakeDamage(1);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy")){
            Flip();
        }
        return;
    }

    private void Die()
    {
        Debug.Log("Slime died!");
        Destroy(gameObject);
    }

    private bool IsGroundAhead()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
    }

    private bool IsWallHit()
    {
        Vector2 direction = movingLeft ? Vector2.left : Vector2.right;
        return Physics2D.Raycast(wallCheck.position, direction, wallCheckDistance, groundLayer);
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
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + direction * wallCheckDistance);
    }
}
