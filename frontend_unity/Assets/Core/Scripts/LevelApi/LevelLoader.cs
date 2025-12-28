using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;

public enum LevelLoadMode
{
    UseCacheIfAvailable,
    ForceDownload
}

public class LevelLoader
{
    public async UniTask<List<PlacedObjectData>> GetLayout(string url, LevelLoadMode levelLoadMode, RuntimeLevelData runtimeLevelData)
    {
        if(levelLoadMode == LevelLoadMode.UseCacheIfAvailable)
        {
            if (runtimeLevelData.cachedLayout != null && runtimeLevelData.cachedLayout.Count > 0)
            {
                Debug.Log($"[LevelSpawner] Using cached layout");
                return runtimeLevelData.cachedLayout;
            }
        }
        else if(levelLoadMode == LevelLoadMode.ForceDownload)
        {
            var result = await LevelApi.Instance.DownloadLevelLayoutAsync(url);

            if (result.Success && result.Data != null)
            {
                runtimeLevelData.cachedLayout = result.Data;
                Debug.Log($"[LevelSpawner] Successfully downloaded layout with {result.Data.Count} objects");
                return result.Data;
            }
            else
            {
                Debug.LogError($"[LevelSpawner] Failed to load layout: {result.Message}");
                return null;
            }
        }
        return null;
    }
}

