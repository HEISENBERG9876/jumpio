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


        var res = await AuthManager.Instance.RegisterAsync(username, email, password);

        if (res.Success)
        {
            UIManager.Instance.ShowMainMenuPanel();
        }
        //TODO : Show error message. Also change the logic entirely, so Register and Login are separate.
    }
}