using UnityEditor.PackageManager;
using UnityEngine;

public class AuthUIController : MonoBehaviour
{
    public GameObject LoginPanel;
    public GameObject MainMenuPanel;

    void Start()
    {
        ShowLogin();
    }

    void OnEnable()
    {
        AuthManager.Instance.OnLoginSuccess += HandleLoginSuccess;
        AuthManager.Instance.OnLoginFailed += HandleLoginFailed;

        AuthManager.Instance.OnRegisterSuccess += HandleRegisterSuccess;
        AuthManager.Instance.OnRegisterFailed += HandleRegisterFailed;
    }

    void OnDisable()
    {
        AuthManager.Instance.OnLoginSuccess -= HandleLoginSuccess;
        AuthManager.Instance.OnLoginFailed -= HandleLoginFailed;

        AuthManager.Instance.OnRegisterSuccess -= HandleRegisterSuccess;
        AuthManager.Instance.OnRegisterFailed -= HandleRegisterFailed;
    }


    private void HandleLoginSuccess()
    {
        ShowMainMenu();
    }

    private void HandleLoginFailed(string msg)
    {
        Debug.LogWarning("[AuthUIController] Login failed" + msg);
    }

    private void HandleRegisterSuccess()
    {
    }

    private void HandleRegisterFailed(string msg)
    {
        Debug.LogWarning("[AuthUIController] Register failed: " + msg);
    }

    private void ShowLogin()
    {
        LoginPanel.SetActive(true);
        MainMenuPanel.SetActive(false);
    }

    public void ShowMainMenu()
    {
        LoginPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }


}
