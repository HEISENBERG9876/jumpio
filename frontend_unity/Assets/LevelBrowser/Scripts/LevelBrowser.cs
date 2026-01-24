using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LevelBrowser : MonoBehaviour
{
    [Header("UI")]
    public Transform levelListContainer;
    public GameObject levelItemPrefab;
    public Button prevButton;
    public Button nextButton;

    [Header("CampaignCreator")]
    public CampaignManager campaignManager;
    public bool selectionMode = false;

    [SerializeField] private Settings settings;
    public Settings Settings => settings;

    private string prevUrl;
    private string nextUrl;

    public async UniTask LoadPageAsync(string url)
    {
        try
        {
            GlobalUIManager.Instance.ShowLoading("Loading level list...");
            var res = await LevelApi.Instance.GetLevelsPageAsync(url);

            if (res.Success)
            {
                GlobalUIManager.Instance.HideLoading();
                PaginatedLevelsResponse page = res.Data;

                foreach (Transform child in levelListContainer)
                {
                    Destroy(child.gameObject);
                }

                foreach (var level in page.results)
                {
                    GameObject item = Instantiate(levelItemPrefab, levelListContainer);

                    if (selectionMode)
                        item.GetComponent<CampaignLevelAddItemUI>().Initialize(level, campaignManager);
                    else
                        item.GetComponent<LevelItemUI>().Initialize(level);
                }

                prevUrl = page.previous;
                nextUrl = page.next;

                prevButton.interactable = !string.IsNullOrEmpty(prevUrl);
                nextButton.interactable = !string.IsNullOrEmpty(nextUrl);
            }
            else
            {
                GlobalUIManager.Instance.ShowInfo("Failed to load level page: " + res.Message);
                Debug.LogError("[LevelBrowser] Failed to load level page: " + res.Message);
            }
        }
        catch
        {
            GlobalUIManager.Instance.ShowInfo("An unexpected error occurred while loading level page.");
            Debug.Log("[LevelBrowser] Unexpected error loading level page");
        }
        finally
        {
            GlobalUIManager.Instance.HideLoading();
        }
    }

    public async void LoadPreviousPage()
    {
        await LoadPageAsync(prevUrl);
    }
    public async void LoadNextPage()
    {
        await LoadPageAsync(nextUrl);
    }
}
