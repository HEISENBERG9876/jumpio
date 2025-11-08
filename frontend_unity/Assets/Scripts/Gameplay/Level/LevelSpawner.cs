using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class LevelSpawner : MonoBehaviour
{
    [Header("References")]
    public PlaceableDatabase database;

    [SerializeField] private RuntimeLevelData runtimeLevelData;

    public async UniTask SpawnLevelFromUrlAsync(string url)
    {
        if(runtimeLevelData.cachedLayout != null && runtimeLevelData.cachedLayout.Count > 0)
        {
            Debug.Log($"[LevelSpawner] Using cached layout");
            SpawnLevelFromList(runtimeLevelData.cachedLayout);
            return;
        }

        var result = await LevelApi.Instance.DownloadLevelLayoutAsync(url);

        if (result.Success && result.Data != null)
        {
            runtimeLevelData.cachedLayout = result.Data;
            Debug.Log($"[LevelSpawner] Successfully downloaded layout with {result.Data.Count} objects");
            SpawnLevelFromList(result.Data);
        }
        else
        {
            Debug.LogError($"[LevelSpawner] Failed to load layout: {result.Message}");
        }
    }

    public void SpawnLevelFromList(List<PlacedObjectData> layout)
    {
        if (database == null || database.placeables == null)
        {
            Debug.LogError("[LevelSpawner] Placeable database not assigned or empty.");
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
            }
            else
            {
                Debug.LogWarning($"[LevelSpawner] Unknown or missing prefab for object ID: {obj.id}");
            }
        }
    }
}
