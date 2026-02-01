using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelItemUI : MonoBehaviour
{
    public RuntimeLevelData runtimeLevelData;
    public TMP_Text titleText;
    public TMP_Text difficultyText;
    public TMP_Text timerText;
    public Button playButton;
    public Button editButton;

    public void Initialize(LevelDataResponse level)
    {
        titleText.text = level.title;
        difficultyText.text = level.difficulty;
        timerText.text = level.timer.ToString();

        playButton.onClick.AddListener(() =>
        {
            runtimeLevelData.title = level.title; //todo add getters/setters
            runtimeLevelData.difficulty = level.difficulty;
            runtimeLevelData.timer = level.timer;
            runtimeLevelData.layoutUrl = level.layout_file;
            Debug.Log("Play level: " + level.title + level.layout_file);
            SceneManager.LoadScene("GameplayScene");
        });

        bool isOwner = AuthManager.Instance.IsLoggedIn && level.user == AuthManager.Instance.CurrentUserId;

        editButton.gameObject.SetActive(isOwner);
        if (isOwner)
        {
            editButton.onClick.AddListener(() =>
            {
                runtimeLevelData.mode = RuntimeLevelMode.Edit;
                runtimeLevelData.levelId = level.id;
                runtimeLevelData.title = level.title;
                runtimeLevelData.difficulty = level.difficulty;
                runtimeLevelData.timer = level.timer;
                runtimeLevelData.layoutUrl = level.layout_file;
                SceneManager.LoadScene("LevelCreatorScene");
            });
        }
    }
}
