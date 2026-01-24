using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CampaignBrowser : MonoBehaviour
{
    [Header("UI")]
    public Transform campaignListContainer;
    public GameObject campaignItemPrefab;
    public Button prevButton;
    public Button nextButton;

    [SerializeField] private Settings settings;
    public Settings Settings => settings;

    private string prevUrl;
    private string nextUrl;

    public async UniTask LoadPageAsync(string url)
    {
        try
        {
            GlobalUIManager.Instance.ShowLoading("Loading campaign list...");
            var res = await CampaignApi.Instance.GetCampaignsPageAsync(url);

            if (!res.Success)
            {
                GlobalUIManager.Instance.ShowInfo("Failed to load campaigns: " + res.Message);
                Debug.LogError("[CampaignBrowser] Failed to load campaign page: " + res.Message);
                return;
            }

            GlobalUIManager.Instance.HideLoading();

            var page = res.Data;

            foreach (Transform child in campaignListContainer)
                Destroy(child.gameObject);

            foreach (var campaign in page.results)
            {
                var item = Instantiate(campaignItemPrefab, campaignListContainer);
                item.GetComponent<CampaignItemUI>().Initialize(campaign);
            }

            prevUrl = page.previous;
            nextUrl = page.next;

            prevButton.interactable = !string.IsNullOrEmpty(prevUrl);
            nextButton.interactable = !string.IsNullOrEmpty(nextUrl);
        }
        catch (System.Exception ex)
        {
            GlobalUIManager.Instance.ShowInfo("Unexpected error while loading campaigns.");
            Debug.LogError("[CampaignBrowser] Unexpected error: " + ex);
        }
        finally
        {
            GlobalUIManager.Instance.HideLoading();
        }
    }

    public async void LoadPreviousPage() => await LoadPageAsync(prevUrl);
    public async void LoadNextPage() => await LoadPageAsync(nextUrl);
}
