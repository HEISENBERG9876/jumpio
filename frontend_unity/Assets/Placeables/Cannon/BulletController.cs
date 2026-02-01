using UnityEngine;

public class BulletController : MonoBehaviour, IKillsPlayer
{
    public LayerMask shouldDestroyOnHitMask;
    public void KillPlayer(PlayerController player)
    {
        player.Die();
        Destroy(gameObject);
    }

    private void Start()
    {
        Destroy(gameObject, 30f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((shouldDestroyOnHitMask.value & (1 << other.gameObject.layer)) != 0)
        {
            Destroy(gameObject);
        }
    }
}
