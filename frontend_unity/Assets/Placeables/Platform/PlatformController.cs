using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public Rigidbody2D rb;
    public Transform wallCheck;
    public float wallCheckDistance = 0.1f;
    public float speed = 4f;
    public LayerMask groundLayer;
    private bool movingLeft = true;
    void FixedUpdate()
    {
        rb.linearVelocityX = movingLeft ? -speed : speed;

        if (IsWallHit())
        {
            Flip();
        }
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
        Gizmos.color = Color.red;
        Vector3 direction = movingLeft ? Vector3.left : Vector3.right;
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + direction * wallCheckDistance);
    }

}
