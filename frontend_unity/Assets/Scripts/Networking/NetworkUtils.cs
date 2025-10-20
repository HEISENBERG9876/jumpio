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
}
