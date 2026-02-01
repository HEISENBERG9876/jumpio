using System;
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
    public RuntimeLevelData runtimeLevelData;
    public TMP_Text saveButtonText;


    private void Start()
    {
        saveButtonText.text = runtimeLevelData.levelId < 0 ? "Save" : "Update";

        if (runtimeLevelData.levelId >= 0)
        {
            titleInput.text = runtimeLevelData.title;
            difficultyText.text = runtimeLevelData.difficulty;
            timerInput.text = runtimeLevelData.timer.ToString();
        }
    }

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
            GlobalUIManager.Instance.ShowLoading(
                runtimeLevelData.levelId < 0 ? "Saving level..." : "Updating level..."
            );

            if (!levelCreator.ValidateLayout())
                return;

            if (!ValidateForm())
                return;

            string title = titleInput.text;
            string difficulty = difficultyText.text;
            int timer = int.Parse(timerInput.text);
            List<PlacedPlaceableData> layout = levelCreator.GetCurrentLayout();

            if (runtimeLevelData.levelId < 0)
            {
                // CREATE
                var res = await LevelApi.Instance.UploadLevelAsync(title, difficulty, timer, layout);

                if (res.Success)
                {
                    runtimeLevelData.levelId = res.Data.id;
                    runtimeLevelData.mode = RuntimeLevelMode.Edit;
                    saveButtonText.text = "Update";

                    GlobalUIManager.Instance.ShowInfo("Level saved successfully!");
                }
                else
                {
                    GlobalUIManager.Instance.ShowInfo(res.Message ?? "Failed to save level");
                }
            }
            else
            {
                // UPDATE
                var res = await LevelApi.Instance.UpdateLevelAsync(
                    runtimeLevelData.levelId,
                    title,
                    difficulty,
                    timer,
                    layout
                );

                if (res.Success)
                {
                    GlobalUIManager.Instance.ShowInfo("Level updated successfully!");
                }
                else
                {
                    GlobalUIManager.Instance.ShowInfo(res.Message ?? "Failed to update level");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            GlobalUIManager.Instance.ShowInfo("Unexpected error while saving level");
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
        if (string.IsNullOrWhiteSpace(difficultyText.text) || difficultyText.text.Length > 8)
        {
            GlobalUIManager.Instance.ShowInfo("Please evaluate difficulty before uploading level");
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
