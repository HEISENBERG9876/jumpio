using UnityEngine;
using UnityEngine.Networking;
using System;
using Cysharp.Threading.Tasks;
using System.Net;

[Serializable]
public class TokenResponse {
    public string access;
    public string refresh;
}

[Serializable]
public class LoginRequest {
    public string username;
    public string password;
}

[Serializable]
public class RegisterRequest {
    public string username;
    public string password;
}

[Serializable]
public class LevelRequest {
    public string level_data;
}

[Serializable]
public class RefreshRequest
{
    public string refresh;
}
public class AuthResult
{
    public bool Success { get; }
    public string Message { get; }
    public long? HttpStatusCode { get; }
    public AuthResult(bool success, string message = null, long? httpStatusCode = null)
    {
        Success = success;
        Message = message;
        HttpStatusCode = httpStatusCode;
    }
}

[System.Serializable]
public class MeResponse
{
    public int id;
}

public class AuthManager : MonoBehaviour
{

    private static AuthManager _instance;
    public static AuthManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<AuthManager>();
                if (_instance == null)
                {
                    Debug.LogError("[AuthManager] Instance not found.");
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private string accessToken;
    private string refreshToken;

    private int currentUserId = -1;
    public int CurrentUserId => currentUserId;

    public bool IsLoggedIn => !string.IsNullOrEmpty(accessToken);
    public string AccessToken => accessToken;

    [SerializeField] private Settings settings;

    public async UniTask<AuthResult> LoginAsync(string username, string password)
    {
        await UniTask.SwitchToMainThread();
        try
        {
            var payload = new LoginRequest { username = username, password = password };

            using (UnityWebRequest www = NetworkUtils.PostJson(settings.baseUserUrl + "token/", payload))
            {
                await www.SendWebRequest().ToUniTask();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    TokenResponse tokens = JsonUtility.FromJson<TokenResponse>(www.downloadHandler.text);

                    accessToken = tokens.access;
                    refreshToken = tokens.refresh;

                    await FetchMeAsync();

                    return new AuthResult(true, "Login successful", www.responseCode);
                }
                else
                {
                    return new AuthResult(false, "Login failed. Invalid login or password", www.responseCode);
                }
            }
        }
        catch(Exception ex)
        {
            Debug.LogError("[AuthManager] Login exception" + ex);
            return new AuthResult(false, "Unexpected error, login failed.");
        }
    }

    public async UniTask<AuthResult> RefreshAccessTokenAsync()
    {
        await UniTask.SwitchToMainThread();
        try
        {
            var payload = new RefreshRequest { refresh = refreshToken };

            using (UnityWebRequest www = NetworkUtils.PostJson(settings.baseUserUrl + "token/refresh/", payload))
            {
                await www.SendWebRequest().ToUniTask();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    TokenResponse tokens = JsonUtility.FromJson<TokenResponse>(www.downloadHandler.text);
                    accessToken = tokens.access;
                    return new AuthResult(true, "Access token refreshed", www.responseCode);
                }
                else
                {
                    accessToken = null;
                    refreshToken = null;
                    Debug.LogWarning("[AuthManager] Refresh token failed: " + www.responseCode + www.downloadHandler?.text + www.error);
                    return new AuthResult(false, "Obtaining refresh token failed", www.responseCode);
                }
            }
        }
        catch(Exception ex)
        {
            Debug.LogError("[AuthManager] Error refreshing access token" + ex);
            return new AuthResult(false, "Unexpected error, refreshing access token failed.");
        }
    }

    public async UniTask<AuthResult> RegisterAsync(string username, string password)
    {
        await UniTask.SwitchToMainThread();
        try
        {
            var payload = new RegisterRequest { username = username, password = password };

            using (UnityWebRequest www = NetworkUtils.PostJson(settings.baseUserUrl + "register/", payload))
            {
                await www.SendWebRequest().ToUniTask();

                if (www.result == UnityWebRequest.Result.Success)
                {

                    var res = await LoginAsync(username, password);
                    if (res.Success)
                    {
                        return new AuthResult(true, "Register succesful. Logged in", www.responseCode);
                    }
                    else
                    {
                        return new AuthResult(false, "Register succeeded, but login failed. " + res.Message, www.responseCode);
                    }
                }
                else
                {
                    return new AuthResult(false, "Register failed. " + www.downloadHandler.text, www.responseCode);
                }
            }
        }
        catch(Exception ex)
        {
            Debug.LogError("[AuthManager] Register exception" + ex);
            return new AuthResult(false, "Unexpected error, register failed.");
        }
    }

    public void Logout()
    {
        accessToken = null;
        refreshToken = null;
        currentUserId = -1;
    }

    private async UniTask FetchMeAsync()
    {
        using (var www = NetworkUtils.GetJson(settings.baseUserUrl + "me/", accessToken))
        {
            await www.SendWebRequest().ToUniTask();
            if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                var me = JsonUtility.FromJson<MeResponse>(www.downloadHandler.text);
                currentUserId = me.id;
            }
            else
            {
                currentUserId = -1;
                Debug.LogWarning("[AuthManager] /me failed: " + www.responseCode + " " + www.downloadHandler.text);
            }
        }
    }

}
