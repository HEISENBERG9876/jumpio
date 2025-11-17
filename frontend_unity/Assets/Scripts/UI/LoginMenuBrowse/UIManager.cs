using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject LoginPanel;
    public GameObject MainMenuPanel;
    public GameObject BrowseMenuPanel;

    private void Awake() => Instance = this;

    private void Start()
    {
        ShowLoginPanel();
    }

    public void ShowLoginPanel()
    {
        LoginPanel.SetActive(true);
        MainMenuPanel.SetActive(false);
        BrowseMenuPanel.SetActive(false);
    }

    public void ShowMainMenuPanel()
    {
        LoginPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
        BrowseMenuPanel.SetActive(false);
    }

    public void ShowBrowsePanel()
    {
        LoginPanel.SetActive(false);
        MainMenuPanel.SetActive(false);
        BrowseMenuPanel.SetActive(true);
    }
}
