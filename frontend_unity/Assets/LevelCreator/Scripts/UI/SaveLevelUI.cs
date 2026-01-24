using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using System;

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

    //NEEDS BETTER VALIDATION AND ERROR MSGS
    public async void OnSaveButtonClicked()
    {

        try
        {
            GlobalUIManager.Instance.ShowLoading("Saving level...");

            string title = titleInput.text;
            string difficulty = difficultyDropdown.options[difficultyDropdown.value].text;
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
        catch (Exception e)
        {
            GlobalUIManager.Instance.ShowInfo("Failed to save level: " + e.Message);
        }
        finally
        {
            GlobalUIManager.Instance.HideLoading();
        }
    }
}
