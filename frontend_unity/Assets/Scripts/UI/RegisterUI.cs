using UnityEngine;
using TMPro;

public class RegisterUI : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public AuthManager authManager;
    public UIManager uiManager;

    public void OnRegisterButtonClicked()
    {
        string username = usernameInput.text;
        string email = emailInput.text;
        string password = passwordInput.text;

        authManager.Register(username, email, password);
    }
}