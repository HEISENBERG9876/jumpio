using UnityEngine.SceneManagement;
using UnityEngine;

public class CampaignCreatorUI : MonoBehaviour
{
    public void OnCreateCampaignButtonClicked()
    {
        SceneManager.LoadScene("CampaignCreatorScene");
    }
}
