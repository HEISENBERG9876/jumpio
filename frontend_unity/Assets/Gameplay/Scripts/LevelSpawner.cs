using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class LevelSpawner : MonoBehaviour
{
    [Header("References")]
    public PlaceableDatabase database;

    public void SpawnLevelFromList(List<PlacedObjectData> layout)
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

        foreach (var obj in layout)
        {
            var placeable = database.placeables.Find(p => p.id == obj.id);
            if (placeable != null && placeable.prefab != null)
            {
                Vector3 position = new Vector3(obj.x, obj.y, 0);
                Quaternion rotation = Quaternion.Euler(0, 0, obj.rotation);
                Instantiate(placeable.prefab, position, rotation);
                Debug.Log($"[LevelSpawner] Spawned object ID: {obj.id} at position: {position} with rotation: {obj.rotation}");
            }
            else
            {
                Debug.LogWarning($"[LevelSpawner] Unknown or missing prefab for object ID: {obj.id}");
            }
        }
    }
}
