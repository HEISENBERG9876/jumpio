using UnityEngine;

public class SpikeController : MonoBehaviour, IKillsPlayer
{
   public void KillPlayer(PlayerController player)
   {
       player.Die();
    }
}
