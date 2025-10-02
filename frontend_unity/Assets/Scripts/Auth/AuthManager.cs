using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

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

public class AuthManager : MonoBehaviour
{

    private static AuthManager _instance;
    public static AuthManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("AuthManager");
                _instance = go.AddComponent<AuthManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    public event Action OnLoginSuccess;
    public event Action<string, string> OnLoginFailed;
    public event Action OnRegisterSuccess;
    public event Action<string, string> OnRegisterFailed;

    private string accessToken;
    private string refreshToken;

    public bool IsLoggedIn => !string.IsNullOrEmpty(accessToken);
    public string AccessToken => accessToken;

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

    public void Login(string username, string password)
    {
        StartCoroutine(LoginCoroutine(username, password));
    }

    private IEnumerator LoginCoroutine(string username, string password)
    {
        var payload = new LoginRequest { username = username, password = password };

        using (UnityWebRequest www = NetworkUtils.PostJson("http://localhost:8000/api/users/token/", payload))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Login response: " + www.downloadHandler.text);
                TokenResponse tokens = JsonUtility.FromJson<TokenResponse>(www.downloadHandler.text);

                accessToken = tokens.access;
                refreshToken = tokens.refresh;

                OnLoginSuccess?.Invoke();

            }
            else
            {
                OnLoginFailed?.Invoke(www.error, www.downloadHandler.text);
            }
        }
    }

    public void RefreshAccessToken()
    {
        StartCoroutine(RefreshAccessTokenCoroutine());
    }

    public IEnumerator RefreshAccessTokenCoroutine()
    {
        var payload = new { refresh = refreshToken };

        using (UnityWebRequest www = NetworkUtils.PostJson("http://localhost:8000/api/users/token/refresh", payload))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                TokenResponse tokens = JsonUtility.FromJson<TokenResponse>(www.downloadHandler.text);
                accessToken = tokens.access;
                Debug.Log("Access token refreshed successfully.");
            }
            else
            {
                accessToken = null;
                refreshToken = null;
                 Debug.LogWarning("Refresh token failed: " + www.error);
            }
        }
        
    }

    public void Register(string username, string email, string password)
    {
        StartCoroutine(RegisterCoroutine(username, email, password));
    }

    private IEnumerator RegisterCoroutine(string username, string email, string password)
    {
        var payload = new RegisterRequest { username = username, email = email, password = password };

        using (UnityWebRequest www = NetworkUtils.PostJson("http://localhost:8000/api/users/register/", payload))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                OnRegisterSuccess?.Invoke();
                Login(username, password);
            }
            else
            {
                OnRegisterFailed?.Invoke(www.error, www.downloadHandler.text);
            }
        }
    }
}
