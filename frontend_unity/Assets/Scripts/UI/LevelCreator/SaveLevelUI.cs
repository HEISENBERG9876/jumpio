using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveLevelUI : MonoBehaviour
{
    public TMP_InputField titleInput;
    public TMP_Dropdown difficultyDropdown;
    public TMP_InputField timerInput;
    public LevelCreator levelCreator;

    public async void OnSaveButtonClicked()
    {
        string title = titleInput.text;
        string difficulty = difficultyDropdown.options[difficultyDropdown.value].text;
        int timer = int.Parse(timerInput.text);
        List<PlacedObjectData> layout = levelCreator.currentLayout;

        await LevelApi.Instance.UploadLevelAsync(title, difficulty, timer, layout);
    }
}
