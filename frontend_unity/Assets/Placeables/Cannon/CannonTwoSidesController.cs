using UnityEngine;

public class CannonTwoSidesController : MonoBehaviour
{
    [SerializeField] Transform leftShootingPoint;
    [SerializeField] Transform rightShootingPoint;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float fireInterval = 1f;
    [SerializeField] float bulletSpeed = 5f;
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
        Debug.Log("Bullets fired from 2 edge cannon");
        GameObject leftBullet = Instantiate(bulletPrefab, leftShootingPoint.position, leftShootingPoint.rotation);
        Rigidbody2D leftRb = leftBullet.GetComponent<Rigidbody2D>();
        if (leftRb != null)
        {
            leftRb.linearVelocity = -leftShootingPoint.right * bulletSpeed;
        }

        GameObject rightBullet = Instantiate(bulletPrefab, rightShootingPoint.position, rightShootingPoint.rotation);
        Rigidbody2D rightRb = rightBullet.GetComponent<Rigidbody2D>();
        if (rightRb != null)
        {
            rightRb.linearVelocity = rightShootingPoint.right * bulletSpeed;
        }
    }

}
