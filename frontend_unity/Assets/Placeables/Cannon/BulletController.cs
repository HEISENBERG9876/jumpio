using UnityEngine;

public class BulletController : MonoBehaviour, IKillsPlayer
{
    public void KillPlayer(PlayerController player)
    {
        Debug.Log("Bullet killed player");
        player.Die();
        Destroy(gameObject);
    }

    private void Start()
    {
        Destroy(gameObject, 10f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
