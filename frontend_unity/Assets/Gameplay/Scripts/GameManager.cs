using UnityEngine;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    void Awake()
    {
        Mode.IsEditorMode = false;
    }

    public enum GameState
    {
        Loading,
        Playing,
        Paused,
        Won,
        Lost
    }

    //TODO allow saving custom player spawn position in settings
    [Header("References")]
    public CinemachineCamera virtualCamera;
    public LevelSpawner levelSpawner;
    public PlayerSpawner playerSpawner;
    [SerializeField] private RuntimeLevelData runtimeLevelData;
    private GameObject player;
    [SerializeField] private Settings settings;

    private async void Start()
    {
        //TODO change load mode depending on where the game is started from
        await StartGame(LevelLoadMode.ForceDownload);
    }

    private void OnEnable()
    {
        FinishFlag.Reached += EndGame;
    }

    private void OnDisable()
    {
        FinishFlag.Reached -= EndGame;
    }

    public async UniTask StartGame(LevelLoadMode levelLoadMode)
    {
        Debug.Log("Game Started");
        List<PlacedObjectData> layout = await new LevelLoader().GetLayout(runtimeLevelData.layoutUrl, levelLoadMode, runtimeLevelData);
        levelSpawner.SpawnLevelFromList(layout);
        player = playerSpawner.SpawnPlayer(settings.playerSpawnPosition, Quaternion.identity);
        virtualCamera.Follow = player.transform;

    }

    public void EndGame()
    {
        Debug.Log("Game Ended - You Win!");
    }


}
