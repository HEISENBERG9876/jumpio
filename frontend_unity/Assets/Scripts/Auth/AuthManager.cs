using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Threading.Tasks;

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
    public string email;
    public string password;
}

[Serializable]
public class LevelRequest {
    public string level_data;
}

public class AuthManager
{

    private static AuthManager _instance;
    public static AuthManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AuthManager();
            }
            return _instance;
        }
    }

    public event Action OnLoginSuccess;
    public event Action<string> OnLoginFailed;
    public event Action OnRegisterSuccess;
    public event Action<string> OnRegisterFailed;

    private string accessToken;
    private string refreshToken;

    public bool IsLoggedIn => !string.IsNullOrEmpty(accessToken);
    public string AccessToken => accessToken;

    public async Task LoginAsync(string username, string password)
    {
        try
        {
            var payload = new LoginRequest { username = username, password = password };

            using (UnityWebRequest www = NetworkUtils.PostJson("http://localhost:8000/api/users/token/", payload))
            {
                var op = www.SendWebRequest();

                while (!op.isDone)
                {
                    await Task.Yield();
                }

                if (www.result == UnityWebRequest.Result.Success)
                {
                    TokenResponse tokens = JsonUtility.FromJson<TokenResponse>(www.downloadHandler.text);

                    accessToken = tokens.access;
                    refreshToken = tokens.refresh;

                    OnLoginSuccess?.Invoke();
                }
                else
                {
                    string errorMessage = ("[AuthManager]" + www.downloadHandler.text + www.error).Trim();
                    OnLoginFailed?.Invoke(errorMessage);
                }
            }
        }
        catch(Exception ex)
        {
            OnLoginFailed?.Invoke("[AuthManager]" + ex.Message);
        }
    }

    public async Task RefreshAccessTokenAsync()
    {
        try
        {
            var payload = new { refresh = refreshToken };

            using (UnityWebRequest www = NetworkUtils.PostJson("http://localhost:8000/api/users/token/refresh", payload))
            {
                var op = www.SendWebRequest();

                while (!op.isDone)
                {
                    await Task.Yield();
                }

                if (www.result == UnityWebRequest.Result.Success)
                {
                    TokenResponse tokens = JsonUtility.FromJson<TokenResponse>(www.downloadHandler.text);
                    accessToken = tokens.access;
                    Debug.Log("[AuthManager] Access token refreshed successfully.");
                }
                else
                {
                    accessToken = null;
                    refreshToken = null;
                    Debug.LogWarning("[AuthManager] Refresh token failed: " + www.error);
                }
            }
        }
        catch(Exception ex)
        {
            Debug.LogError("[AuthManager] Error refreshing access token" + ex);
        }
    }

    public async Task RegisterAsync(string username, string email, string password)
    {
        try
        {
            var payload = new RegisterRequest { username = username, email = email, password = password };

            using (UnityWebRequest www = NetworkUtils.PostJson("http://localhost:8000/api/users/register/", payload))
            {
                var op = www.SendWebRequest();

                while (!op.isDone)
                {
                    await Task.Yield();
                }

                if (www.result == UnityWebRequest.Result.Success)
                {
                    OnRegisterSuccess?.Invoke();
                    await LoginAsync(username, password);
                }
                else
                {
                    string errorMessage = ("[AuthManager]" + www.downloadHandler.text + www.error).Trim();
                    OnRegisterFailed?.Invoke(errorMessage);
                }
            }
        }
        catch(Exception ex)
        {
            OnRegisterFailed?.Invoke("[AuthManager]" + ex.Message);
        }
    }
}
