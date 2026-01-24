using UnityEngine;
using TMPro;

public class RegisterUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;

    public async void OnRegisterButtonClicked()
    {
        string username = usernameInput.text;
        string email = emailInput.text;
        string password = passwordInput.text;

        GlobalUIManager.Instance.ShowLoading("Registering...");
        try
        {
            var res = await AuthManager.Instance.RegisterAsync(username, email, password);
            if (res.Success)
            {
                UIManager.Instance.ShowMainMenuPanel();
            }
            else
            {
                GlobalUIManager.Instance.ShowInfo(res.Message ?? "Registration failed");
            } 
        }
        finally
        {
                GlobalUIManager.Instance.HideLoading();
        }

    }
}