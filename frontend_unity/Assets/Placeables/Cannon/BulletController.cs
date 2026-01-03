using UnityEngine;

public class BulletController : MonoBehaviour, IDamageDealer
{
    public void DealDamage(PlayerController player)
    {
        Debug.Log("Bullet dealing damage to player");
        player.TakeDamage(100);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
