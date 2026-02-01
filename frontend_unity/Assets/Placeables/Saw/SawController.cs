using UnityEngine;

public class SawController : MonoBehaviour, IKillsPlayer
{
    public void KillPlayer(PlayerController player)
    {
        player.Die();
    }

    private void Update()
    {
        transform.Rotate(0, 0, 360 * Time.deltaTime);
    }
}
