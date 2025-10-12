using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LevelBrowser : MonoBehaviour
{
    [Header("UI")]
    public Transform levelListContainer;
    public GameObject levelItemPrefab;
    public Button prevButton;
    public Button nextButton;

    private string prevUrl;
    private string nextUrl;

    private async void OnEnable()
    {
        await LoadPage("http://localhost:8000/api/levels/");
    }

    public async Task LoadPage(string url)
    {
        try
        {
            PaginatedLevelsResponse page = await LevelApi.Instance.GetLevelsPageAsync("http://localhost:8000/api/levels/");

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
        catch (System.Exception ex)
        {
            Debug.Log("Error loading levels in LevelBrowser" + ex);
        }
    }
    public async void LoadPreviousPage()
    {
        await LoadPage(prevUrl);
    }
    public async void LoadNextPage()
    {
        await LoadPage(nextUrl);
    }
}
