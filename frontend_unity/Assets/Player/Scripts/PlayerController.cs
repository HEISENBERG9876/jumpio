using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private int health = 1;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.TryGetComponent<IStompable>(out var stompable))
        {
            stompable.OnStomped(this, collision);
        }

        else if(collision.gameObject.TryGetComponent<IDamageDealer>(out var damageDealer))
        {
            damageDealer.DealDamage(this);
        }
    }

    public void TakeDamage(int amount)
    {
        health -= 1;
        if(health <= 0)
        {
            Die();
        }
            Debug.Log($"Player took {amount} damage.");
    }

    private void Die()
    {
        Debug.Log("Player died!");
    }
}
