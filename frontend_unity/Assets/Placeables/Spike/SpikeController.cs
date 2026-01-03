using UnityEngine;

public class SpikeController : MonoBehaviour, IDamageDealer
{
   public void DealDamage(PlayerController player)
   {
       Debug.Log("Spike dealing damage to player");
       player.TakeDamage(100);
    }
}
