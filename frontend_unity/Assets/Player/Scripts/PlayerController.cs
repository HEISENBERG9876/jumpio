using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public event Action Died;
    private int health = 100;
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IDamageDealer>(out var damageDealer))
        {
            damageDealer.DealDamage(this);
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if(health <= 0)
        {
            Die();
        }
            Debug.Log($"Player took {amount} damage.");
    }

    private void Die()
    {
        Died?.Invoke();
        Debug.Log("Player died!");
    }
}
