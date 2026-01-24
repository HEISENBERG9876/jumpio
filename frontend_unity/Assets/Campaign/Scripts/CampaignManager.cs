using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CampaignManager : MonoBehaviour
{
    [Header("Saving campaign")]
    public TMP_InputField titleInput;
    public TMP_InputField descriptionInput;

    [Header("Selected levels")]
    public Transform selectedLevelListContainer;
    public GameObject selectedItemPrefab;
    private readonly List<LevelDataResponse> selectedLevels = new();

    [Header("Level browser")]
    public LevelBrowser levelBrowser;

    [Header("Settings")]
    [SerializeField] private Settings settings;

    public async void Start()
    {
        await LoadLevelsPageAsync(settings.baseLevelUrl);
        RenderCampaignList();
    }

    public async UniTask LoadLevelsPageAsync(string url)
    {
        await levelBrowser.LoadPageAsync(url);
    }

    public void AddLevel(LevelDataResponse level)
    {
        for (int i = 0; i < selectedLevels.Count; i++)
        {
            if (selectedLevels[i].id == level.id)
            {
                return;
            }
        }

        selectedLevels.Add(level);
        RenderCampaignList();
    }

    public void RemoveLevel(int index)
    {
        if (index < 0 || index >= selectedLevels.Count)
        {
            return;
        }

        selectedLevels.RemoveAt(index);
        RenderCampaignList();
    }

    public void MoveLevel(int index, int delta)
    {
        int newIndex = index + delta;
        if (index < 0 || index >= selectedLevels.Count)
        {
            return;
        }

        if (newIndex < 0 || newIndex >= selectedLevels.Count)
        {
            return;
        }

        var temp = selectedLevels[index];
        selectedLevels[index] = selectedLevels[newIndex];
        selectedLevels[newIndex] = temp;

        RenderCampaignList();
    }

    public void ClearCampaign()
    {
        selectedLevels.Clear();
        titleInput.text = "";
        descriptionInput.text = "";
        RenderCampaignList();
    }

    private void RenderCampaignList()
    {
        foreach (Transform child in selectedLevelListContainer)
            Destroy(child.gameObject);

        for (int i = 0; i < selectedLevels.Count; i++)
        {
            var go = Instantiate(selectedItemPrefab, selectedLevelListContainer);

            bool canUp = i > 0;
            bool canDown = i < selectedLevels.Count - 1;

            go.GetComponent<SelectedCampaignLevelItemUI>()
              .Initialize(selectedLevels[i], i, this, canUp, canDown);
        }
    }

    public async void SaveCampaign()
    {
        try
        {
            string title = titleInput.text?.Trim();
            string description = descriptionInput.text?.Trim();

            if (string.IsNullOrEmpty(title))
            {
                Debug.LogWarning("[CampaignCreator] Title required");
                return;
            }

            if (selectedLevels.Count == 0)
            {
                Debug.LogWarning("[CampaignCreator] Campaign must contain at least one level");
                return;
            }

            GlobalUIManager.Instance.ShowLoading("Saving campaign...");

            var createRes = await CampaignApi.Instance.CreateCampaignAsync(title, description);
            if (!createRes.Success)
            {
                GlobalUIManager.Instance.ShowInfo("Failed to save campaign: " + createRes.Message);
                Debug.LogError("[CampaignCreator] Failed to create campaign: " + createRes.Message);
                return;
            }

            string campaignId = createRes.Data.id;

            var orderedIds = new List<string>(selectedLevels.Count);
            foreach (var lvl in selectedLevels)
            {
                orderedIds.Add(lvl.id.ToString());
            }

            var setRes = await CampaignApi.Instance.SetCampaignLevelsAsync(campaignId, orderedIds);
            if (!setRes.Success)
            {
                GlobalUIManager.Instance.ShowInfo("Failed to set campaign levels: " + setRes.Message);
                Debug.LogError("[CampaignCreator] Failed to set campaign levels: " + setRes.Message);
                return;
            }

            Debug.Log("[CampaignCreator] Campaign saved. id=" + campaignId);
        }
        catch (System.Exception ex)
        {
            GlobalUIManager.Instance.ShowInfo("Unexpected exception while saving campaign");
            Debug.LogError("[CampaignCreator] Exception while saving campaign: " + ex);
        }
        finally
        {
            GlobalUIManager.Instance.HideLoading();
        }
    }
}
