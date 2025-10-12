using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;

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
    public async Task UploadLevelAsync(string title, string difficulty, int timer, List<PlacedObjectData> layout)
    {
        var body = new LevelPostBody
        {
            title = title,
            difficulty = difficulty,
            timer = timer,
            layout = layout
        };

        string debugJson = JsonUtility.ToJson(body, true);
        Debug.Log("UPLOAD JSON:\n" + debugJson);

        using (UnityWebRequest www = NetworkUtils.PostJson("http://localhost:8000/api/levels/", body, AuthManager.Instance.AccessToken))
        {
            var op = www.SendWebRequest();

            while (!op.isDone)
                await Task.Yield();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Level uploaded successfully");
            }
            else
            {
                Debug.LogError("Error uploading level:" + www.downloadHandler.text);
                throw new Exception(www.downloadHandler.text);
            }
        }
    }

    //TODO better error handling to avoid game crashing
    //TODO convert every web-related coroutine to use tasks, because coroutines are frame-dependant.
    public async Task<LevelDataResponse> DownloadLevelAsync(int id)
    {
        string url = $"http://localhost:8000/api/levels/{id}";
        using (var www = NetworkUtils.GetJson(url, AuthManager.Instance.AccessToken))
        {
            var op = www.SendWebRequest();
            while (!op.isDone)
            {
                await Task.Yield();
            }

            if (www.result == UnityWebRequest.Result.Success)
            {
                return JsonUtility.FromJson<LevelDataResponse>(www.downloadHandler.text);
            }
            else
            {
                string body = www.downloadHandler.text;
                throw new Exception("Request failed:" + www.downloadHandler.text);
            }
        }
    }

    public async Task<PaginatedLevelsResponse> GetLevelsPageAsync(string url = "http://localhost:8000/api/levels/")
    {
        using (var www = NetworkUtils.GetJson(url, AuthManager.Instance.AccessToken))
        {
            var op = www.SendWebRequest();

            while (!op.isDone)
            {
                await Task.Yield();
            }

            if (www.result == UnityWebRequest.Result.Success)
            {
                return JsonUtility.FromJson<PaginatedLevelsResponse>(www.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error fetching levels: " + www.downloadHandler.text);
                throw new Exception("Request failed:" + www.downloadHandler.text);
            }
        }
    }


}
