using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelItemUI : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text difficultyText;
    public TMP_Text timerText;
    public Button playButton;

    private LevelDataResponse layout;

    public void Initialize(LevelDataResponse level)
    {
        titleText.text = level.title;
        difficultyText.text = level.difficulty;
        timerText.text = level.timer.ToString();
        //TODO: not load level layouts in browser, but upon playing
        layout = level;

        playButton.onClick.AddListener(() =>
        {
            Debug.Log($"Playing level {level.id}: {level.title}");
        });
    }
}
