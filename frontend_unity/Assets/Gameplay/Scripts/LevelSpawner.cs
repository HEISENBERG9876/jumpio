using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class LevelSpawner : MonoBehaviour
{
    [Header("References")]
    public PlaceableDatabase database;

    public void SpawnLevelFromList(List<PlacedPlaceableData> layout)
    {
        if (database == null || database.placeables == null)
        {
            Debug.LogError("[LevelSpawner] Placeable database not assigned or empty.");
            return;
        }

        if(layout == null || layout.Count == 0)
        {
            Debug.LogWarning("[LevelSpawner] Layout is null or empty. No objects to spawn.");
            return;
        }

        foreach (PlacedPlaceableData placedObjectData in layout)
        {
            Placeable placeable = database.placeables.Find(p => p.id == placedObjectData.id);
            if (placeable != null && placeable.prefab != null)
            {
                Vector3 position = new Vector3(placedObjectData.x + 0.5f, placedObjectData.y + 0.5f, 0f);
                Quaternion rotation = Quaternion.Euler(0, 0, placedObjectData.rotation);
                Instantiate(placeable.prefab, position, rotation);
                Debug.Log($"[LevelSpawner] Spawned object ID: {placedObjectData.id} at position: {position} with rotation: {placedObjectData.rotation}");
            }
            else
            {
                Debug.LogWarning($"[LevelSpawner] Unknown or missing prefab for object ID: {placedObjectData.id}");
            }
        }
    }
}
