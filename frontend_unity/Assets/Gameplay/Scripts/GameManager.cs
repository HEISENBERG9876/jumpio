using UnityEngine;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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
    [SerializeField] RuntimeCampaignData runtimeCampaignData;
    private GameObject player;
    private PlayerController playerController;
    [SerializeField] Settings settings;
    [SerializeField] GameplayUI gameplayUI;
    private GameState currentState;
    public Transform levelRoot;

    private async void Start()
    {
        if (testLevelData != null && testLevelData.hasData)
        {
            StartTestGame();
            return;
        }
        else if (runtimeCampaignData.hasData)
        {
            await StartCampaignLevel(LevelLoadMode.ForceDownload);
        }
        else
        {
            await StartGame(LevelLoadMode.ForceDownload);
        }

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
        levelSpawner.SpawnLevelFromList(layout, levelRoot, new(0, 0));
        player = playerSpawner.SpawnPlayer(layout, levelRoot); //needs checks if SpawnMarker exists
        playerController = player.GetComponent<PlayerController>();
        playerController.Died += OnLose;
        playerController.Finished += OnWin;
        virtualCamera.Follow = player.transform;

        currentState = GameState.Playing;
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

    private async UniTask StartCampaignLevel(LevelLoadMode mode)
    {
        gameplayUI.UpdateReturnToCreatorButton(false);
        gameplayUI.HideAllPanels();
        currentState = GameState.Loading;

        int i = runtimeCampaignData.currentIndex;

        runtimeLevelData.layoutUrl = runtimeCampaignData.levelLayoutUrls[i];
        runtimeLevelData.timer = runtimeCampaignData.timers[i];

        await StartGame(mode);
    }

    public void OnWin()
    {
        Time.timeScale = 0f;
        timer.StopTimer();
        currentState = GameState.Won;

        if (runtimeCampaignData.hasData)
        {
            bool hasNext = runtimeCampaignData.currentIndex + 1 < runtimeCampaignData.levelLayoutUrls.Count;
            if (hasNext)
            {
                gameplayUI.ShowAdvanceLevelPanel();
            }
            else
            {
                gameplayUI.ShowCampaignFinishedPanel();
            }

            return;
        }

        if (testLevelData.hasData)
        {
            gameplayUI.ShowTestLevelWonPanel();
            return;
        }

        gameplayUI.ShowLevelWonPanel();
    }

    public void OnLose()
    {
        Time.timeScale = 0f;
        timer.StopTimer();
        gameplayUI.showLevelLostPanel();
        currentState = GameState.Lost;
    }

    public void ReturnToBrowser() //stopped working and backs to menu. not a big issue
    {
        Time.timeScale = 1f;
        MenuReturnState.ReturnToLevelBrowser = true;
        testLevelData?.Clear();
        runtimeCampaignData?.Clear();
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

        if (testLevelData.hasData)
        {
            StartTestGame();
        }
        else if (runtimeCampaignData.hasData)
        {
            await StartCampaignCurrentLevel(LevelLoadMode.ForceDownload);
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

    //campaign

    public async void NextLevel()
    {
        if (!runtimeCampaignData.hasData)
        {
            return;
        }

        runtimeCampaignData.currentIndex++;

        if (runtimeCampaignData.currentIndex >= runtimeCampaignData.levelLayoutUrls.Count)
        {
            runtimeCampaignData.Clear();
            ReturnToBrowser();
            return;
        }

        Time.timeScale = 1f;
        ResetRun();
        await StartCampaignCurrentLevel(LevelLoadMode.ForceDownload);
    }

    public void QuitCampaign()
    {
        runtimeCampaignData?.Clear();
        ReturnToBrowser();
    }

    public async void ReplayCampaign()
    {
        runtimeCampaignData.hasData = true;
        runtimeCampaignData.currentIndex = 0;

        Time.timeScale = 1f;
        ResetRun();
        await StartCampaignCurrentLevel(LevelLoadMode.ForceDownload);
    }

    private async UniTask StartCampaignCurrentLevel(LevelLoadMode mode)
    {
        int i = runtimeCampaignData.currentIndex;

        runtimeLevelData.layoutUrl = runtimeCampaignData.levelLayoutUrls[i];
        runtimeLevelData.timer = runtimeCampaignData.timers[i];

        await StartGame(mode);
    }
}
