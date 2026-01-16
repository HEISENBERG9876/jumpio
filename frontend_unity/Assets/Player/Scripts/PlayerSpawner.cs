using UnityEngine;
using System.Collections.Generic;

//TODO should be part of controller
public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject SpawnPlayer(List<PlacedPlaceableData> layout, Transform levelRoot) //gotta sort vector2/vector2int mess
    {
        Vector2 spawnPosition = FindSpawnLocal(layout); //could be more optimal if stored
        GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity, levelRoot);
        player.transform.localPosition = spawnPosition; //might be not needed
        return player; //might need further changes
    }

    public static Vector2 FindSpawnLocal(List<PlacedPlaceableData> layout)
    {
        foreach (PlacedPlaceableData placeable in layout)
        {
            if (placeable.id == "SpawnMarker")
            {
                return new Vector2(placeable.x, placeable.y);
            }
        }
        return Vector2.zero;
    }
}
