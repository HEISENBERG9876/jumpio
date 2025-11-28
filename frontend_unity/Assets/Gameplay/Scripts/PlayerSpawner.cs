using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject SpawnPlayer(Vector2 SpawnPosition, Quaternion rotation)
    {
        GameObject player = Instantiate(playerPrefab, SpawnPosition, Quaternion.identity);
        return player;
    }
}
