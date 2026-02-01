using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking; 

public class NetworkUtils
{
    public static UnityWebRequest PostJson(string url, object payload, string authToken = null)
    {
        string json = JsonUtility.ToJson(payload);
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        if (!string.IsNullOrEmpty(authToken))
        {
            request.SetRequestHeader("Authorization", "Bearer " + authToken);
        }
        return request;
    }

    public static UnityWebRequest GetJson(string url, string authToken = null)
    {
        var request = UnityWebRequest.Get(url);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Accept", "application/json");
        if (!string.IsNullOrEmpty(authToken))
        {
            request.SetRequestHeader("Authorization", "Bearer " + authToken);
        }
        return request;
    }

    public static UnityWebRequest PostMultipart(
        string url,
        string authToken,
        IEnumerable<(string key, string value)> formFields,
        IEnumerable<(string fieldName, byte[] fileData, string fileName, string mimeType)> files)
    {
        var form = new WWWForm();

        foreach (var (key, value) in formFields)
            form.AddField(key, value);

        foreach (var file in files)
            form.AddBinaryData(file.fieldName, file.fileData, file.fileName, file.mimeType);

        var request = UnityWebRequest.Post(url, form);
        request.downloadHandler = new DownloadHandlerBuffer();

        if (!string.IsNullOrEmpty(authToken))
            request.SetRequestHeader("Authorization", "Bearer " + authToken);

        return request;
    }

    public static UnityWebRequest PatchMultipart(
    string url,
    string authToken,
    IEnumerable<(string key, string value)> formFields,
    IEnumerable<(string fieldName, byte[] fileData, string fileName, string mimeType)> files)
    {
        var form = new WWWForm();

        foreach (var (key, value) in formFields)
            form.AddField(key, value);

        foreach (var file in files)
            form.AddBinaryData(file.fieldName, file.fileData, file.fileName, file.mimeType);

        var request = UnityWebRequest.Post(url, form);
        request.method = "PATCH";
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Accept", "application/json");

        if (!string.IsNullOrEmpty(authToken))
            request.SetRequestHeader("Authorization", "Bearer " + authToken);

        return request;
    }

    public static UnityWebRequest PutJson(string url, object payload, string authToken = null)
    {
        string json = JsonUtility.ToJson(payload);
        var request = new UnityWebRequest(url, "PUT");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");
        if (!string.IsNullOrEmpty(authToken))
        {
            request.SetRequestHeader("Authorization", "Bearer " + authToken);
        }
        return request;
    }


    public static async UniTask<UnityWebRequest> SendWithAutoRefreshAsync(Func<UnityWebRequest> requestToBuild)
    {
        await UniTask.SwitchToMainThread();

        UnityWebRequest request1 = null;

        try
        {
            request1 = requestToBuild();
            await request1.SendWebRequest().ToUniTask();
        }
        catch (UnityWebRequestException ex)
        {
            request1 = ex.UnityWebRequest;
        }

        if (request1 != null && request1.result == UnityWebRequest.Result.Success)
        {
            return request1;
        }

        if (request1 == null || request1.responseCode != 401)
        {
            return request1;
        }

        request1.Dispose();

        var refresh = await AuthManager.Instance.RefreshAccessTokenAsync();
        if (!refresh.Success)
        {
            return requestToBuild();
        }

        UnityWebRequest request2 = null;

        try
        {
            request2 = requestToBuild();
            await request2.SendWebRequest().ToUniTask();
        }
        catch (UnityWebRequestException ex)
        {
            request2 = ex.UnityWebRequest;
        }

        return request2;
    }
}
