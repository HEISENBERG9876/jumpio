using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CampaignLevelAddItemUI : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text difficultyText;
    public TMP_Text timerText;
    public Button addButton;

    public void Initialize(LevelDataResponse level, CampaignManager manager)
    {
        titleText.text = level.title;
        difficultyText.text = level.difficulty;
        timerText.text = level.timer.ToString();

        addButton.onClick.RemoveAllListeners();
        addButton.onClick.AddListener(() => manager.AddLevel(level));
    }
}
