using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedCampaignLevelItemUI : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text otherText;
    public Button upButton;
    public Button downButton;
    public Button removeButton;

    public void Initialize(LevelDataResponse level, int index, CampaignManager manager, bool canUp, bool canDown)
    {
        titleText.text = level.title;
        otherText.text = $"{level.difficulty} | {level.timer}s";

        upButton.interactable = canUp;
        downButton.interactable = canDown;

        upButton.onClick.RemoveAllListeners();
        downButton.onClick.RemoveAllListeners();
        removeButton.onClick.RemoveAllListeners();

        upButton.onClick.AddListener(() => manager.MoveLevel(index, -1));
        downButton.onClick.AddListener(() => manager.MoveLevel(index, +1));
        removeButton.onClick.AddListener(() => manager.RemoveLevel(index));
    }
}
