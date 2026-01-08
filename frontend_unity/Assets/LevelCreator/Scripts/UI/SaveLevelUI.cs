using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveLevelUI : MonoBehaviour
{
    public TMP_InputField titleInput;
    public TMP_Dropdown difficultyDropdown;
    public TMP_InputField timerInput;
    public LevelCreator levelCreator;
    public GameObject SaveLevelFormPanel;


    public void OnSaveFormOpenClicked()
    {
        SaveLevelFormPanel.SetActive(true);
    }

    public void OnCancelButtonClicked()
    {
        SaveLevelFormPanel.SetActive(false);
    }

    public async void OnSaveButtonClicked()
    {
        string title = titleInput.text;
        string difficulty = difficultyDropdown.options[difficultyDropdown.value].text; //TODO automatic. And proably better as int
        int timer = int.Parse(timerInput.text);
        List<PlacedPlaceableData> layout = levelCreator.GetCurrentLayout();

        await LevelApi.Instance.UploadLevelAsync(title, difficulty, timer, layout);
    }
}
