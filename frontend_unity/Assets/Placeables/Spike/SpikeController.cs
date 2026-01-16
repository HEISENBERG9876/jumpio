using UnityEngine;

public class SpikeController : MonoBehaviour, IKillsPlayer
{
   public void KillPlayer(PlayerController player)
   {
       Debug.Log("Spike killed player");
       player.Die();
    }
}
