using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class CampaignApiResult
{
    public bool Success { get; }
    public string Message { get; }
    public long? HttpStatusCode { get; }

    public CampaignApiResult(bool success, string message = null, long? httpStatusCode = null)
    {
        Success = success;
        Message = message;
        HttpStatusCode = httpStatusCode;
    }
}

public class CampaignApiResult<T> : CampaignApiResult
{
    public T Data { get; }

    public CampaignApiResult(bool success, string message = null, long? httpStatusCode = null, T data = default)
        : base(success, message, httpStatusCode)
    {
        Data = data;
    }
}


[System.Serializable]
public class CampaignCreateRequest
{
    public string title;
    public string description;
}

[System.Serializable]
public class CampaignLevelItem
{
    public string level_id;
    public int order_index;
}

[System.Serializable]
public class CampaignSetLevelsRequest
{
    public List<CampaignLevelItem> levels;
}

[System.Serializable]
public class CampaignLevelReadResponse
{
    public int order_index;
    public string level_id;
    public string title;
    public string difficulty;
    public int timer;
    public string layout_file;
}

[System.Serializable]
public class CampaignResponse
{
    public string id;
    public string title;
    public string user;
    public string description;
    public string date_created;
    public List<CampaignLevelReadResponse> levels;
}

[System.Serializable]
public class PaginatedCampaignsResponse
{
    public int count;
    public string next;
    public string previous;
    public List<CampaignResponse> results;
}

public class CampaignApi
{
    private static CampaignApi _instance;
    public static CampaignApi Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new CampaignApi();
            }
            return _instance;
        }
    }

    private CampaignApi()
    {
        settings = Resources.Load<Settings>("Settings");
    }

    private Settings settings;
    private string BaseCampaignUrl => settings.baseCampaignUrl;

    public async UniTask<CampaignApiResult<CampaignResponse>> CreateCampaignAsync(string title, string description = null)
    {
        await UniTask.SwitchToMainThread();

        var payload = new CampaignCreateRequest
        {
            title = title,
            description = description
        };

        using (var www = await NetworkUtils.SendWithAutoRefreshAsync(() =>
            NetworkUtils.PostJson(BaseCampaignUrl, payload, AuthManager.Instance.AccessToken)))
        {
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("[CampaignApi] Campaign created: " + www.downloadHandler.text);
                var campaign = JsonUtility.FromJson<CampaignResponse>(www.downloadHandler.text);
                return new CampaignApiResult<CampaignResponse>(true, "Campaign created successfully", www.responseCode, campaign);
            }
            else
            {
                Debug.LogError("[CampaignApi] Error creating campaign: " + www.downloadHandler.text);
                return new CampaignApiResult<CampaignResponse>(false, "Failed to create campaign: " + www.downloadHandler.text, www.responseCode);
            }
        }
    }

    public async UniTask<CampaignApiResult<CampaignResponse>> GetCampaignAsync(string campaignId)
    {
        await UniTask.SwitchToMainThread();

        string url = $"{BaseCampaignUrl}{campaignId}/";

        using (var www = await NetworkUtils.SendWithAutoRefreshAsync(() =>
            NetworkUtils.GetJson(url, AuthManager.Instance.AccessToken)))
        {
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("[CampaignApi] Campaign downloaded: " + www.downloadHandler.text);
                var campaign = JsonUtility.FromJson<CampaignResponse>(www.downloadHandler.text);
                return new CampaignApiResult<CampaignResponse>(true, "Campaign downloaded successfully", www.responseCode, campaign);
            }
            else
            {
                Debug.LogError("[CampaignApi] Error downloading campaign: " + www.downloadHandler.text);
                return new CampaignApiResult<CampaignResponse>(false, "Failed to download campaign: " + www.downloadHandler.text, www.responseCode);
            }
        }
    }

    public async UniTask<CampaignApiResult<PaginatedCampaignsResponse>> GetCampaignsPageAsync(string url)
    {
        await UniTask.SwitchToMainThread();

        using (var www = await NetworkUtils.SendWithAutoRefreshAsync(() =>
            NetworkUtils.GetJson(url, AuthManager.Instance.AccessToken)))
        {
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("[CampaignApi] Campaigns page downloaded: " + www.downloadHandler.text);
                var page = JsonUtility.FromJson<PaginatedCampaignsResponse>(www.downloadHandler.text);
                return new CampaignApiResult<PaginatedCampaignsResponse>(true, "Campaign page downloaded successfully", www.responseCode, page);
            }
            else
            {
                Debug.LogError("[CampaignApi] Error fetching campaigns: " + www.downloadHandler.text);
                return new CampaignApiResult<PaginatedCampaignsResponse>(false, "Failed to download campaign page: " + www.downloadHandler.text, www.responseCode);
            }
        }
    }

    public async UniTask<CampaignApiResult<CampaignResponse>> SetCampaignLevelsAsync(string campaignId, List<string> orderedLevelIds)
    {
        await UniTask.SwitchToMainThread();

        string url = $"{BaseCampaignUrl}{campaignId}/levels/";

        var items = new List<CampaignLevelItem>(orderedLevelIds.Count);
        for (int i = 0; i < orderedLevelIds.Count; i++)
        {
            items.Add(new CampaignLevelItem
            {
                level_id = orderedLevelIds[i],
                order_index = i
            });
        }

        var payload = new CampaignSetLevelsRequest { levels = items };

        using (var www = await NetworkUtils.SendWithAutoRefreshAsync(() =>
            NetworkUtils.PutJson(url, payload, AuthManager.Instance.AccessToken)))
        {
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("[CampaignApi] Campaign levels set: " + www.downloadHandler.text);
                var campaign = JsonUtility.FromJson<CampaignResponse>(www.downloadHandler.text);
                return new CampaignApiResult<CampaignResponse>(true, "Campaign levels set successfully", www.responseCode, campaign);
            }
            else
            {
                Debug.LogError("[CampaignApi] Error setting campaign levels: " + www.downloadHandler.text);
                return new CampaignApiResult<CampaignResponse>(false, "Failed to set campaign levels: " + www.downloadHandler.text, www.responseCode);
            }
        }
    }
}
