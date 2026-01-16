using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public event Action Died;
    public event Action Finished;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IStompable stompable = collision.collider.GetComponentInParent<IStompable>();
        if (stompable != null)
        {
            stompable.OnStomped(this, collision);
            return;
        }

        IKillsPlayer killer = collision.collider.GetComponentInParent<IKillsPlayer>();
        if (killer != null)
        {
            killer.KillPlayer(this);
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IKillsPlayer killer = other.GetComponentInParent<IKillsPlayer>();
        if (killer != null)
        {
            killer.KillPlayer(this);
        }
    }

    public void Die()
    {
        Died?.Invoke();
        Debug.Log("Player died!");
    }

    public void Finish()
    {
        Finished?.Invoke();
        Debug.Log("Player finished!");
    }
}
