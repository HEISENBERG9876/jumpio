using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class LevelApiResult
{
    public bool Success { get; }
    public string Message { get; }
    public long? HttpStatusCode { get; }
    public LevelApiResult(bool success, string message = null, long? httpStatusCode = null)
    {
        Success = success;
        Message = message;
        HttpStatusCode = httpStatusCode;
    }
}

public class LevelApiResult<T> : LevelApiResult
{
    public T Data { get; }
    public LevelApiResult(bool success, string message = null, long? httpStatusCode = null, T data = default) 
        : base(success, message, httpStatusCode)
    {
        Data = data;
    }
}


[System.Serializable]
public class LayoutWrapper
{
    public List<PlacedPlaceableData> layout;
}

public class LevelApi
{
    private static LevelApi _instance;
    public static LevelApi Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LevelApi();
            }
            return _instance;

        }
    }
    private LevelApi() {
        settings = Resources.Load<Settings>("Settings");
    }

    private Settings settings;

    //TODO multipart form data for file uploads
    public async UniTask<LevelApiResult<LevelDataResponse>> UploadLevelAsync(
        string title,
        string difficulty,
        int timer,
        List<PlacedPlaceableData> layout,
        Texture2D previewImage = null)
    {
        await UniTask.SwitchToMainThread();

        using (UnityWebRequest www = await NetworkUtils.SendWithAutoRefreshAsync(() =>
        {
            var fields = new List<(string, string)>
        {
            ("title", title),
            ("difficulty", difficulty),
            ("timer", timer.ToString())
        };

            var wrapper = new LayoutWrapper { layout = layout };
            string layoutJson = JsonUtility.ToJson(wrapper);
            byte[] layoutBytes = Encoding.UTF8.GetBytes(layoutJson);

            var files = new List<(string, byte[], string, string)>
        {
            ("layout_file", layoutBytes, "layout.json", "application/json")
        };

            if (previewImage != null)
            {
                byte[] imageBytes = previewImage.EncodeToPNG();
                files.Add(("preview_image", imageBytes, "preview.png", "image/png"));
            }

            return NetworkUtils.PostMultipart(settings.baseLevelUrl, AuthManager.Instance.AccessToken, fields, files.ToArray());
        }))
        {
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("[LevelApi] Level uploaded: " + www.downloadHandler.text);

                var created = JsonUtility.FromJson<LevelDataResponse>(www.downloadHandler.text);

                return new LevelApiResult<LevelDataResponse>(
                    true,
                    "Level uploaded successfully",
                    www.responseCode,
                    created
                );
            }
            else
            {
                Debug.LogError("[LevelApi] Error uploading level:" + www.downloadHandler.text);
                return new LevelApiResult<LevelDataResponse>(
                    false,
                    "Failed to upload level: " + www.downloadHandler.text,
                    www.responseCode,
                    null
                );
            }
        }
    }



    public async UniTask<LevelApiResult<List<PlacedPlaceableData>>> DownloadLevelLayoutAsync(string url)
    {
        await UniTask.SwitchToMainThread();

        using (var www = NetworkUtils.GetJson(url))
        {
            await www.SendWebRequest().ToUniTask();

            if (www.result == UnityWebRequest.Result.Success)
            {
                var wrapper = JsonUtility.FromJson<LayoutWrapper>(www.downloadHandler.text);
                Debug.Log("[LevelApi] Level downloaded: " + www.downloadHandler.text);
                return new LevelApiResult<List<PlacedPlaceableData>>(true, "Level downloaded successfully", www.responseCode, wrapper.layout);
            }
            else
            {
                Debug.LogError("[LevelApi] Request failed:" + www.downloadHandler.text);
                return new LevelApiResult<List<PlacedPlaceableData>>(false, "Failed to download level: " + www.downloadHandler.text, www.responseCode);
            }
        }
    }

    public async UniTask<LevelApiResult<PaginatedLevelsResponse>> GetLevelsPageAsync(string url)
    {
        await UniTask.SwitchToMainThread();
        Debug.Log("[LevelApi] Token: " + AuthManager.Instance.AccessToken);

        using (var www = await NetworkUtils.SendWithAutoRefreshAsync(() =>
        NetworkUtils.GetJson(url, AuthManager.Instance.AccessToken)))
        {
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("[LevelApi] Levels page downloaded: " + www.downloadHandler.text);
                return new LevelApiResult<PaginatedLevelsResponse>(true, "Level page downloaded succesfully", www.responseCode, JsonUtility.FromJson<PaginatedLevelsResponse>(www.downloadHandler.text));
            }
            else
            {
                Debug.LogError("[LevelApi] Error fetching levels: " + www.downloadHandler.text);
                return new LevelApiResult<PaginatedLevelsResponse>(false, "Failed to download level page: " + www.downloadHandler.text, www.responseCode);
            }
        }          
    }

    public async UniTask<LevelApiResult> UpdateLevelAsync(
        int id,
        string title,
        string difficulty,
        int timer,
        List<PlacedPlaceableData> layout)
    {
        await UniTask.SwitchToMainThread();

        using (UnityWebRequest www = await NetworkUtils.SendWithAutoRefreshAsync(() =>
        {
            var fields = new List<(string, string)>
        {
            ("title", title),
            ("difficulty", difficulty),
            ("timer", timer.ToString())
        };

            var wrapper = new LayoutWrapper { layout = layout };
            string layoutJson = JsonUtility.ToJson(wrapper);
            byte[] layoutBytes = Encoding.UTF8.GetBytes(layoutJson);

            var files = new List<(string, byte[], string, string)>
        {
            ("layout_file", layoutBytes, "layout.json", "application/json")
        };

            string url = $"{settings.baseLevelUrl}{id}/";
            return NetworkUtils.PatchMultipart(url, AuthManager.Instance.AccessToken, fields, files.ToArray());
        }))
        {
            if (www.result == UnityWebRequest.Result.Success)
            {
                return new LevelApiResult(true, "Level updated successfully", www.responseCode);
            }

            return new LevelApiResult(false, "Failed to update level: " + www.downloadHandler.text, www.responseCode);
        }
    }



}
