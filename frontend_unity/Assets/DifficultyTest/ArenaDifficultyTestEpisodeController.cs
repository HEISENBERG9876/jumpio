using System.Collections.Generic;
using UnityEngine;
public class ArenaDifficultyTestEpisodeController : MonoBehaviour
{
    [Header("Arena")]
    public Transform levelRoot;
    public Transform agentRoot;

    [Header("Refs")]
    public LevelSpawner levelSpawner;
    public PlayerAgent agent;
    public DifficultyTestData testData;

    private List<PlacedPlaceableData> layout => testData != null && testData.hasData ? testData.layout : null;


    private void Awake()
    {
        if (agent != null)
        {
            agent.testArena = this;
        }
    }


    public void StartEpisode()
    {
        if (layout == null || layout.Count == 0)
        {
            Debug.LogError("[DifficultyTestEpisodeController] Layout is null or empty");
            return;
        }

        levelSpawner.Clear(levelRoot);
        levelSpawner.SpawnLevelFromListLocal(layout, levelRoot);

        Vector2 spawnLocal = PlayerSpawner.FindSpawnLocal(layout);
        spawnLocal.x += 0.5f;
        spawnLocal.y += 0.5f;

        agent.ResetForNewEpisode(spawnLocal);
        agent.testArena = this;
    }
}
