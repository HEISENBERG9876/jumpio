using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class LoginUI : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    public async void OnLoginButtonClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        await AuthManager.Instance.LoginAsync(username, password);
    }
}