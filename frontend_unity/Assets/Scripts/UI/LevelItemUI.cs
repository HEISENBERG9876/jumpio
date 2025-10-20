using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelItemUI : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text difficultyText;
    public TMP_Text timerText;
    public Button playButton;

    public void Initialize(LevelDataResponse level)
    {
        titleText.text = level.title;
        difficultyText.text = level.difficulty;
        timerText.text = level.timer.ToString();

        playButton.onClick.AddListener(() =>
        {
            Debug.Log("Play level: " + level.title + level.layout_file);
        });
    }
}
