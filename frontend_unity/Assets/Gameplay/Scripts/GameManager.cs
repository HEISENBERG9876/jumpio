using UnityEngine;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

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
        await StartGame();
    }

    public async UniTask StartGame()
    {
        Debug.Log("Game Started");
        await levelSpawner.SpawnLevelFromUrlAsync(runtimeLevelData.layoutUrl);
        player = playerSpawner.SpawnPlayer(settings.playerSpawnPosition, Quaternion.identity);
        virtualCamera.Follow = player.transform;

    }


}
