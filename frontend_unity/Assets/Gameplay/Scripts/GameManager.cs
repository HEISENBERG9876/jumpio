using UnityEngine;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Loading,
        Playing,
        Paused,
        Won,
        Lost
    }

    //TODO allow saving custom player spawn position in settings OR make player a placeable
    [Header("References")]
    public CinemachineCamera virtualCamera;
    public LevelSpawner levelSpawner;
    public PlayerSpawner playerSpawner;
    public Timer timer;
    [SerializeField] RuntimeLevelData runtimeLevelData;
    private GameObject player;
    private PlayerController playerController;
    [SerializeField] Settings settings;
    [SerializeField] GameplayUI gameplayUI;
    private GameState currentState;

    private async void Start()
    {
        await StartGame(LevelLoadMode.ForceDownload);
    }

    private void OnEnable()
    {
        FinishFlag.Reached += OnWin;
        timer.TimerEnded += OnLose;
    }

    private void OnDisable()
    {
        FinishFlag.Reached -= OnWin;
        timer.TimerEnded -= OnLose;

        if (playerController != null)
        {
            playerController.Died -= OnLose;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Playing)
            {
                Pause();
            }
            else if (currentState == GameState.Paused)
            {
                Resume();
            }
        }
    }

    public async UniTask StartGame(LevelLoadMode levelLoadMode)
    {
        gameplayUI.HideAllPanels();
        currentState = GameState.Loading;

        List<PlacedPlaceableData> layout = await new LevelLoader().GetLayout(runtimeLevelData.layoutUrl, levelLoadMode, runtimeLevelData);
        timer.StartTimer(runtimeLevelData.timer);
        levelSpawner.SpawnLevelFromList(layout);
        player = playerSpawner.SpawnPlayer(settings.playerSpawnPosition, Quaternion.identity);
        playerController = player.GetComponent<PlayerController>();
        playerController.Died += OnLose;
        virtualCamera.Follow = player.transform;

        currentState= GameState.Playing;

    }

    public void OnWin()
    {
        Time.timeScale = 0f;
        timer.StopTimer();
        gameplayUI.ShowWonPanel();
        currentState= GameState.Won;
    }

    public void OnLose()
    {
        Time.timeScale = 0f;
        timer.StopTimer();
        gameplayUI.showLostPanel();
        currentState= GameState.Lost;
    }

    public void ReturnToBrowser()
    {
        Time.timeScale = 1f;
        MenuReturnState.ReturnToBrowser = true;
        SceneManager.LoadScene("LoginMenuBrowseScene");
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        gameplayUI.ShowPausePanel();
        currentState = GameState.Paused;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        gameplayUI.HideAllPanels();
        currentState = GameState.Playing;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
