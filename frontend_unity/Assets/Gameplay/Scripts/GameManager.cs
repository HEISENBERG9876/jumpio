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

    public CinemachineCamera virtualCamera;
    public LevelSpawner levelSpawner;
    public PlayerSpawner playerSpawner;
    public Timer timer;
    [SerializeField] RuntimeLevelData runtimeLevelData;
    [SerializeField] TestLevelData testLevelData;
    private GameObject player;
    private PlayerController playerController;
    [SerializeField] Settings settings;
    [SerializeField] GameplayUI gameplayUI;
    private GameState currentState;
    public Transform levelRoot;

    private async void Start()
    {
        if(testLevelData != null && testLevelData.hasPayload)
        {
            StartTestGame();
            return;
        }

        await StartGame(LevelLoadMode.ForceDownload);
    }

    private void OnEnable()
    {
        timer.TimerEnded += OnLose;
    }

    private void OnDisable()
    {
        timer.TimerEnded -= OnLose;

        if (playerController != null)
        {
            playerController.Died -= OnLose;
            playerController.Finished -= OnWin;
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
        gameplayUI.UpdateReturnToCreatorButton(false);
        gameplayUI.HideAllPanels();
        currentState = GameState.Loading;

        List<PlacedPlaceableData> layout = await new LevelLoader().GetLayout(runtimeLevelData.layoutUrl, levelLoadMode, runtimeLevelData);
        timer.StartTimer(runtimeLevelData.timer);
        levelSpawner.SpawnLevelFromList(layout, levelRoot, new(0,0));
        player = playerSpawner.SpawnPlayer(layout, levelRoot); //needs checks if SpawnMarker exists
        playerController = player.GetComponent<PlayerController>();
        playerController.Died += OnLose;
        playerController.Finished += OnWin;
        virtualCamera.Follow = player.transform;

        currentState= GameState.Playing;
    }

    private void StartTestGame()
    {
        gameplayUI.UpdateReturnToCreatorButton(true);
        gameplayUI.HideAllPanels();
        currentState = GameState.Loading;
        List<PlacedPlaceableData> layout = testLevelData.layout;
        timer.StartTimer(testLevelData.timer);
        levelSpawner.SpawnLevelFromList(layout, levelRoot, new(0, 0));
        player = playerSpawner.SpawnPlayer(layout, levelRoot);
        playerController = player.GetComponent<PlayerController>();
        playerController.Died += OnLose;
        playerController.Finished += OnWin;
        virtualCamera.Follow = player.transform;

        currentState = GameState.Playing;
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
        testLevelData?.Clear();
        SceneManager.LoadScene("LoginMenuBrowseScene", LoadSceneMode.Single);
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

    public async void Restart()
    {
        Time.timeScale = 1f;
        ResetRun();

        if (testLevelData.hasPayload)
        {
            StartTestGame();
        }
        else
        {
            await StartGame(LevelLoadMode.ForceDownload);
        }
    }
    public async void ReturnToCreator()
    {
        Time.timeScale = 1f;
        testLevelData?.Clear();

        await SceneManager.UnloadSceneAsync("GameplayScene");
        CreatorSession.Instance.SetCreatorActive(true);
    }

    private void ResetRun()
    {
        if (playerController != null)
        {
            playerController.Died -= OnLose;
            playerController.Finished -= OnWin;
            playerController = null;
        }

        timer.StopTimer();
        gameplayUI.HideAllPanels();

        if (levelSpawner != null && levelRoot != null)
            levelSpawner.Clear(levelRoot);

        player = null;
        virtualCamera.Follow = null;
    }


}
