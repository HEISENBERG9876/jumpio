using UnityEngine;

public class FlyingSlimeController : MonoBehaviour, IStompable, IKillsPlayer
{
    [Header("Movement")]
    [SerializeField] private float speed = 4f;
    [SerializeField] private float maxTravelDistance = 4f;

    [Header("Checks")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckDistance = 0.1f;

    [SerializeField] private LayerMask obstacleMask;

    [Header("Death")]
    [SerializeField] private Animator animator;

    private bool movingLeft = true;
    private Vector2 travelStartPos;
    private bool dead;
    [SerializeField] private float deathGravity = 3f;
    [SerializeField] private float destroyAfterSeconds = 1.0f;

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        rb.gravityScale = 0f;
        travelStartPos = rb.position;
    }

    private void FixedUpdate()
    {
        if (dead)
        {
            return;
        }

        rb.linearVelocityX = movingLeft ? -speed : speed;

        if (IsFrontHit())
        {
            Flip();
            return;
        }

        if (Vector2.Distance(travelStartPos, rb.position) >= maxTravelDistance)
        {
            Flip();
            return;
        }
    }

    private bool IsFrontHit()
    {
        Vector2 direction = movingLeft ? Vector2.left : Vector2.right;

        if (Physics2D.Raycast(wallCheck.position, direction, wallCheckDistance, obstacleMask))
            return true;

        return false;
    }

    private void Flip()
    {
        movingLeft = !movingLeft;
        travelStartPos = rb.position;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void OnStomped(PlayerController player, Collision2D collision)
    {
        if (dead)
        {
            return;
        }

        if (collision.contacts.Length > 0 && collision.contacts[0].normal.y > 0.5f)
        {
            Die();
        }
        else
        {
            KillPlayer(player);
        }
    }

    public void KillPlayer(PlayerController player)
    {
        if (dead)
        {
            return;
        }
        player.Die();
    }

    private void Die()
    {
        dead = true;

        foreach (var c in GetComponentsInChildren<Collider2D>())
        {
            c.enabled = false;
        }

        rb.simulated = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = deathGravity;
        rb.freezeRotation = true;
        rb.linearVelocity = Vector2.zero;

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        Destroy(gameObject, destroyAfterSeconds);
    }


    public void OnDeathAnimationEnd()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (wallCheck == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Vector3 dir = movingLeft ? Vector3.left : Vector3.right;
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + dir * wallCheckDistance);
    }
}
