using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class LevelSpawner : MonoBehaviour
{
    [Header("References")]
    public PlaceableDatabase database;

    public void Clear(Transform levelRoot)
    {
        if (levelRoot == null) return;
        for (int i = levelRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(levelRoot.GetChild(i).gameObject);
        }
    }

    public void SpawnLevelFromList(List<PlacedPlaceableData> layout, Transform levelRoot, Vector2Int worldOffset)
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
                Vector3 position = new Vector3(placedObjectData.x + 0.5f + worldOffset.x, 
                    placedObjectData.y + 0.5f + placeable.offsetY + worldOffset.y, 
                    0f); //better to save cell size somewhere else than hard code
                Quaternion rotation = Quaternion.Euler(0, 0, placedObjectData.rotation);
                Instantiate(placeable.prefab, position, rotation, levelRoot);
            }
            else
            {
                Debug.LogWarning($"[LevelSpawner] Unknown or missing prefab for object ID: {placedObjectData.id}");
            }
        }
    }

    public void SpawnLevelFromListLocal(List<PlacedPlaceableData> layout, Transform levelRoot)
    {
        foreach (PlacedPlaceableData placedObjectData in layout)
        {
            Placeable placeable = database.placeables.Find(p => p.id == placedObjectData.id);
            if (placeable != null && placeable.prefab != null)
            {
                Vector3 localPosition = new Vector3(
                    placedObjectData.x + 0.5f,
                    placedObjectData.y + 0.5f + placeable.offsetY,
                    0f
                );

                var gameObject = Instantiate(placeable.prefab, levelRoot);

                gameObject.transform.localPosition = localPosition;

                gameObject.transform.localRotation = Quaternion.Euler(0, 0, placedObjectData.rotation);
            }
        }
    }

    public static Transform FindFinish(Transform levelRoot)
    {
        if (levelRoot == null)
        {
            return null;
        }

        var finishFlag = levelRoot.GetComponentInChildren<FinishFlag>();
        if (finishFlag == null)
        {
            Debug.LogWarning("[LevelSpawner] FinishFlag not found in levelRoot.");
        }
        Debug.Log("[LevelSpawner] FinishFlag found in levelRoot." + finishFlag.transform);

        return (finishFlag != null) ? finishFlag.transform : null;
    }

}
