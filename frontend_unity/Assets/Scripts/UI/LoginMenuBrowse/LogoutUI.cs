using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoutUI : MonoBehaviour
{
    public void OnLogoutButtonClicked()
    {
        AuthManager.Instance.Logout();
        UIManager.Instance.ShowLoginPanel();
    }
}
