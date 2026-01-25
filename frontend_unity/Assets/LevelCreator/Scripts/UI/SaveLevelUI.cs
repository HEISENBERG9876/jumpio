using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveLevelUI : MonoBehaviour
{
    public TMP_InputField titleInput;
    public TMP_Text difficultyText;
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

        try
        {
            GlobalUIManager.Instance.ShowLoading("Saving level...");

            if (!levelCreator.ValidateLayout())
            {
                return;
            }
            if (!ValidateForm())
            {
                return;
            }

            string title = titleInput.text;
            string difficulty = difficultyText.text;
            int timer = int.Parse(timerInput.text);
            List<PlacedPlaceableData> layout = levelCreator.GetCurrentLayout();

            var res = await LevelApi.Instance.UploadLevelAsync(title, difficulty, timer, layout);

            if (res.Success)
            {
                GlobalUIManager.Instance.ShowInfo("Level saved successfully!");
            }
            else
            {
                GlobalUIManager.Instance.ShowInfo(res.Message ?? "Failed to save level");
            }
        }
        catch
        {
            GlobalUIManager.Instance.ShowInfo("Failed to save level, unexpected error");
        }
        finally
        {
            GlobalUIManager.Instance.HideLoading();
        }
    }

    public bool ValidateForm()
    {
        if (string.IsNullOrWhiteSpace(titleInput.text))
        {
            GlobalUIManager.Instance.ShowInfo("Title cannot be empty");
            return false;
        }
        if (string.IsNullOrWhiteSpace(difficultyText.text))
        {
            GlobalUIManager.Instance.ShowInfo("Difficulty cannot be empty");
            return false;
        }
        if (!int.TryParse(timerInput.text, out int timer) || timer < 0)
        {
            GlobalUIManager.Instance.ShowInfo("Timer must be a valid non-negative integer");
            return false;
        }
        return true;
    }


}
