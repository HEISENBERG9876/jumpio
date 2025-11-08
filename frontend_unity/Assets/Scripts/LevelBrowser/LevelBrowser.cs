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

    [SerializeField] private Settings settings;

    private string prevUrl;
    private string nextUrl;

    private async void OnEnable()
    {
        await LoadPageAsync(settings.baseLevelUrl);
    }

    //TODO app needs a base URL so I don't have to hardcode it. Also async void can be avoided. Also pooling for destroying game objects.
    public async UniTask LoadPageAsync(string url)
    {
        try
        {
            var res = await LevelApi.Instance.GetLevelsPageAsync(url);
            if (res.Success)
            {
                PaginatedLevelsResponse page = res.Data;

                foreach (Transform child in levelListContainer)
                {
                    Destroy(child.gameObject);
                }

                foreach (var level in page.results)
                {
                    GameObject item = Instantiate(levelItemPrefab, levelListContainer);
                    item.GetComponent<LevelItemUI>().Initialize(level);
                }

                prevUrl = page.previous;
                nextUrl = page.next;

                prevButton.interactable = !string.IsNullOrEmpty(prevUrl);
                nextButton.interactable = !string.IsNullOrEmpty(nextUrl);
            }
            else
            {
                //TODO show error to user
                Debug.LogError("[LevelBrowser] Failed to load level page: " + res.Message);
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("[LevelBrowser] Unexpected error loading level page" + ex);
        }
    }

    //TODO wire these up in unity
    public async void LoadPreviousPage()
    {
        await LoadPageAsync(prevUrl);
    }
    public async void LoadNextPage()
    {
        await LoadPageAsync(nextUrl);
    }
}
