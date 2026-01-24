using UnityEngine;
using TMPro;

public class LoginUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    public async void OnLoginButtonClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        GlobalUIManager.Instance.ShowLoading("Logging in...");
        try
        {
            var res = await AuthManager.Instance.LoginAsync(username, password);

            if (res.Success)
            {
                UIManager.Instance.ShowMainMenuPanel();
            }
            else
            {
                GlobalUIManager.Instance.ShowInfo(res.Message ?? "Login failed");
            }
        }
        finally
        {
            GlobalUIManager.Instance.HideLoading();
        }
    }
}