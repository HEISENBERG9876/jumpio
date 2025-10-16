using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class RegisterUI : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;

    public async void OnRegisterButtonClicked()
    {
        string username = usernameInput.text;
        string email = emailInput.text;
        string password = passwordInput.text;

        await AuthManager.Instance.RegisterAsync(username, email, password);
    }
}