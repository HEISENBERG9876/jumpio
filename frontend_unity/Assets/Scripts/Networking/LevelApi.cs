using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

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
    private LevelApi() { }
    public async UniTask<LevelApiResult> UploadLevelAsync(string title, string difficulty, int timer, List<PlacedObjectData> layout)
    {
        await UniTask.SwitchToMainThread();

        var body = new LevelPostBody
        {
            title = title,
            difficulty = difficulty,
            timer = timer,
            layout = layout
        };

        using (UnityWebRequest www = NetworkUtils.PostJson("http://localhost:8000/api/levels/", body, AuthManager.Instance.AccessToken))
        {
            await www.SendWebRequest().ToUniTask();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("[LevelApi] Level uploaded: " + www.downloadHandler.text);
                return new LevelApiResult(true, "Level uploaded successfully", www.responseCode);
            }
            else
            {
                Debug.LogError("[LevelApi] Error uploading level:" + www.downloadHandler.text);
                return new LevelApiResult(false, "Failed to upload level: " + www.downloadHandler.text, www.responseCode);
            }
        }
    }

    public async UniTask<LevelApiResult<LevelDataResponse>> DownloadLevelAsync(int id)
    {
        await UniTask.SwitchToMainThread();

        string url = $"http://localhost:8000/api/levels/{id}";

        using (var www = NetworkUtils.GetJson(url, AuthManager.Instance.AccessToken))
        {
            await www.SendWebRequest().ToUniTask();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("[LevelApi] Level downloaded: " + www.downloadHandler.text);
                return new LevelApiResult<LevelDataResponse>(true, "Level downloaded successfully", www.responseCode, JsonUtility.FromJson<LevelDataResponse>(www.downloadHandler.text));
            }
            else
            {
                Debug.LogError("[LevelApi] Request failed:" + www.downloadHandler.text);
                return new LevelApiResult<LevelDataResponse>(false, "Failed to download level: " + www.downloadHandler.text, www.responseCode);
            }
        }
    }

    public async UniTask<LevelApiResult<PaginatedLevelsResponse>> GetLevelsPageAsync(string url = "http://localhost:8000/api/levels/")
    {
        await UniTask.SwitchToMainThread();

        using (var www = NetworkUtils.GetJson(url, AuthManager.Instance.AccessToken))
        {
            await www.SendWebRequest().ToUniTask();

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


}
