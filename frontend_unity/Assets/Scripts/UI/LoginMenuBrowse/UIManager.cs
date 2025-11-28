using UnityEngine;
using Cysharp.Threading.Tasks;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject LoginPanel;
    public GameObject MainMenuPanel;
    public GameObject BrowseMenuPanel; 

    private void Awake() => Instance = this;

    private async void Start()
    {
        if(MenuReturnState.ReturnToBrowser)
        {
            MenuReturnState.ReturnToBrowser = false;
            await ShowBrowsePanel();
        }
        else if (AuthManager.Instance.IsLoggedIn)
        {
            ShowMainMenuPanel();
        }
        else
        {
            ShowLoginPanel();
        }
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

    public async UniTask ShowBrowsePanel()
    {
        LoginPanel.SetActive(false);
        MainMenuPanel.SetActive(false);
        BrowseMenuPanel.SetActive(true);

        var browser = BrowseMenuPanel.GetComponentInChildren<LevelBrowser>();
        await browser.LoadPageAsync(browser.Settings.baseLevelUrl);
    }
}
