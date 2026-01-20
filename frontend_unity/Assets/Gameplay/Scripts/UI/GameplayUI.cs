using UnityEngine;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject wonPanel;
    [SerializeField] GameObject lostPanel;
    [SerializeField] GameObject advanceLevelPanel;
    [SerializeField] GameObject campaignFinishedPanel;


    [SerializeField] GameObject returnToEditorButton;
    [SerializeField] GameManager gameManager;

    //no need for so many panels, better to add conditionals that slightly change  existing
    public void HideAllPanels()
    {
        pausePanel.SetActive(false);
        wonPanel.SetActive(false);
        lostPanel.SetActive(false);
        advanceLevelPanel.SetActive(false);
        campaignFinishedPanel.SetActive(false);
    }

    public void ShowPausePanel()
    {
        HideAllPanels();
        pausePanel.SetActive(true);
    }

    public void ShowLevelWonPanel()
    {
        HideAllPanels();
        wonPanel.SetActive(true);
    }

    public void showLevelLostPanel()
    {
        HideAllPanels();
        lostPanel.SetActive(true);
    }

    public void ShowAdvanceLevelPanel()
    {
        HideAllPanels();
        advanceLevelPanel.SetActive(true);
    }

    public void ShowCampaignFinishedPanel()
    {
        HideAllPanels();
        campaignFinishedPanel.SetActive(true);
    }

    public void UpdateReturnToCreatorButton(bool isInTestMode)
    {
        returnToEditorButton.SetActive(isInTestMode);
    }


}
