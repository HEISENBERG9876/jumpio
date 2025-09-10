using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject LoginPanel;
    public GameObject MainMenuPanel;

    void Start()
    {
        ShowLogin();
    }

    public void ShowLogin()
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
