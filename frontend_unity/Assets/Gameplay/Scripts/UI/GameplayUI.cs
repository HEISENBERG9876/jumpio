using UnityEngine;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject wonPanel;
    [SerializeField] GameObject lostPanel;
    [SerializeField] GameObject returnToEditorButton;
    [SerializeField] GameManager gameManager;

    public void HideAllPanels()
    {
        pausePanel.SetActive(false);
        wonPanel.SetActive(false);
        lostPanel.SetActive(false);
    }

    public void ShowPausePanel()
    {
        HideAllPanels();
        pausePanel.SetActive(true);
    }

    public void ShowWonPanel()
    {
        HideAllPanels();
        wonPanel.SetActive(true);
    }

    public void showLostPanel()
    {
        HideAllPanels();
        lostPanel.SetActive(true);
    }

    public void UpdateReturnToCreatorButton(bool isInTestMode)
    {
        returnToEditorButton.SetActive(isInTestMode);
    }


}
