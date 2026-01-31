using UnityEngine;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject wonPanel;
    [SerializeField] GameObject lostPanel;
    [SerializeField] GameObject advanceLevelPanel;
    [SerializeField] GameObject testLevelWonPanel;
    [SerializeField] GameObject testLevelLostPanel;
    [SerializeField] GameObject testPausePanel;
    [SerializeField] GameObject campaignFinishedPanel;

    [SerializeField] GameManager gameManager;

    //no need for so many panels, better to add conditionals that slightly change  existing
    public void HideAllPanels()
    {
        pausePanel.SetActive(false);
        wonPanel.SetActive(false);
        lostPanel.SetActive(false);
        advanceLevelPanel.SetActive(false);
        campaignFinishedPanel.SetActive(false);
        testLevelWonPanel.SetActive(false);
        testLevelLostPanel.SetActive(false);
        testPausePanel.SetActive(false);
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

    public void ShowTestLevelWonPanel()
    {
        HideAllPanels();
        testLevelWonPanel.SetActive(true);
    }

    public void ShowTestLevelLostPanel()
    {
        HideAllPanels();
        testLevelLostPanel.SetActive(true);
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

    public void ShowTestPausePanel()
    {
        HideAllPanels();
        testPausePanel.SetActive(true);
    }
}
