using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[System.Serializable]
public class TokenResponse {
    public string access;
    public string refresh;
}

[System.Serializable]
public class LoginRequest {
    public string username;
    public string password;
}

[System.Serializable]
public class RegisterRequest {
    public string username;
    public string email;
    public string password;
}

[System.Serializable]
public class LevelRequest {
    public string level_data;
}

public class AuthManager : MonoBehaviour
{
    public UIManager uiManager;

    private string accessToken;
    private string refreshToken;

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

                uiManager.ShowMainMenu();
            }
            else
            {
                Debug.LogError("Login failed: " + www.error + " " + www.downloadHandler.text);
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
                Debug.Log("Register successful: " + www.downloadHandler.text);
                Login(username, password);
            }
            else
            {
                Debug.LogError("Register failed: " + www.error + " " + www.downloadHandler.text);
            }
        }
    }

    public IEnumerator CreateLevel(string levelData)
    {
        var payload = new LevelRequest { level_data = levelData };

        using (UnityWebRequest www = NetworkUtils.PostJson("http://localhost:8000/api/levels/", payload, accessToken))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Level created: " + www.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Create level failed: " + www.error + " " + www.downloadHandler.text);
            }
        }
    }
}
