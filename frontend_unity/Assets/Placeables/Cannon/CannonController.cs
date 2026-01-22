using UnityEngine;

public class CannonController : MonoBehaviour
{
    [SerializeField] Transform leftShootingPoint;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float fireInterval = 1f;
    [SerializeField] float bulletSpeed = 4f;
    private float fireTimer;

    private void Update()
    {
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireInterval)
        {
            FireBullets();
            fireTimer = 0f;
        }
    }

    private void FireBullets()
    {
        GameObject leftBullet = Instantiate(bulletPrefab, leftShootingPoint.position, leftShootingPoint.rotation, transform);
        Rigidbody2D leftRb = leftBullet.GetComponent<Rigidbody2D>();
        if (leftRb != null)
        {
            leftRb.linearVelocity = -leftShootingPoint.right * bulletSpeed;
        }
    }

}
