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

    private void HandleLoginFailed(string error, string text)
    {
        Debug.LogError("Login failed: " + error + " " + text);
    }

    private void HandleRegisterSuccess()
    {
        //nothing for now- Registering succesfully automatically logs in, so that the main menu is being shown.
    }

    private void HandleRegisterFailed(string error, string text)
    {
        Debug.LogError("Register failed: " + error + " " + text);
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
