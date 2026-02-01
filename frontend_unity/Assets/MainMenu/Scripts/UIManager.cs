using UnityEngine;
using Cysharp.Threading.Tasks;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject LoginPanel;
    public GameObject MainMenuPanel;
    public GameObject BrowseMenuPanel;
    public GameObject BrowseLevelsPanel;
    public GameObject BrowseCampaignsPanel;


    private void Awake() => Instance = this;

    private async void Start()
    {
        if(MenuReturnState.ReturnToLevelBrowser)
        {
            Debug.Log("Returning to Level Browser");
            MenuReturnState.ReturnToLevelBrowser = false;
            await ShowBrowseLevelsPanel();
        }
        else if (MenuReturnState.ReturnToCampaignBrowser)
        {
            MenuReturnState.ReturnToCampaignBrowser = false;
            await ShowBrowseCampaignsPanel();
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

   public async UniTask ShowBrowseLevelsPanel()
{
    LoginPanel.SetActive(false);
    MainMenuPanel.SetActive(false);
    BrowseMenuPanel.SetActive(true);

    BrowseLevelsPanel.SetActive(true);
    BrowseCampaignsPanel.SetActive(false);

    var browser = BrowseLevelsPanel.GetComponentInChildren<LevelBrowser>();
    await browser.LoadPageAsync(browser.Settings.baseLevelUrl);
}

public async UniTask ShowBrowseCampaignsPanel()
{
    LoginPanel.SetActive(false);
    MainMenuPanel.SetActive(false);
    BrowseMenuPanel.SetActive(true);

    BrowseLevelsPanel.SetActive(false);
    BrowseCampaignsPanel.SetActive(true);

    var browser = BrowseCampaignsPanel.GetComponentInChildren<CampaignBrowser>();
    await browser.LoadPageAsync(browser.Settings.baseCampaignUrl);
}

    public async void OnBrowseLevelsClicked()
    {
        await ShowBrowseLevelsPanel();
    }
    public async void OnBrowseCampaignsClicked()
    {
        await ShowBrowseCampaignsPanel();
    }
    public void Exit()
    {
        Application.Quit();
    }
}

