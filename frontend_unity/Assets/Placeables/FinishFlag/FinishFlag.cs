using UnityEngine;

public class FinishFlag : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.TryGetComponent<PlayerController>(out var playerController))
        {
            playerController.Finish();
        }

        if (other.TryGetComponent<PlayerAgent>(out var agent))
            agent.Win();
    }
}
