using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CampaignItemUI : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public TMP_Text otherText; //num of levels, might add difficulty, author etc
    public Button playButton;
    public RuntimeCampaignData runtimeCampaignData;

    private CampaignResponse campaign;

    public void Initialize(CampaignResponse campaign)
    {
        this.campaign = campaign;

        titleText.text = campaign.title;
        descriptionText.text = campaign.description;
        otherText.text = $"{(campaign.levels != null ? campaign.levels.Count : 0)} levels";

        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(() =>
        {
            PlayCampaign().Forget();
        });
    }

    private async UniTaskVoid PlayCampaign()
    {
        var result = await CampaignApi.Instance.GetCampaignAsync(campaign.id);
        if (!result.Success)
        {
            Debug.LogError("[CampaignItemUI] Failed to load campaign: " + result.Message);
            return;
        }

        var full = result.Data;

        runtimeCampaignData.Clear();
        runtimeCampaignData.hasData = true;
        runtimeCampaignData.campaignId = full.id;
        runtimeCampaignData.campaignTitle = full.title;
        runtimeCampaignData.currentIndex = 0;

        foreach (var level in full.levels)
        {
            runtimeCampaignData.levelLayoutUrls.Add(level.layout_file);
            runtimeCampaignData.timers.Add(level.timer);
        }

        SceneManager.LoadScene("GameplayScene");
    }


}
