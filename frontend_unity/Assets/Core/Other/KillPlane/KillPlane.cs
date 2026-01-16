using UnityEngine;

public class KillPlane : MonoBehaviour, IKillsPlayer
{
    public void KillPlayer(PlayerController player)
    {
        player.Die();
    }
}
