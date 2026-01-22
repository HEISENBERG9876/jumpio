using System.Collections.Generic;
using UnityEngine;

public class ArenaEpisodeController : MonoBehaviour
{
    [Header("Generation")]
    public ChunkDatabase chunkDatabase;
    public int chunksToPlace = 6;
    public int baseSeed = 1234;

    [Header("Arena")]
    public Transform levelRoot;
    public Transform agentRoot;
    public Vector2Int worldOffset;
    public Vector2Int startCell = Vector2Int.zero;

    [Header("Refs")]
    public LevelSpawner levelSpawner;
    public PlayerAgent agent;

    private int episodeCounter = 0;

    private void Awake()
    {
        if (agent != null)
        {
            agent.arena = this;
        }
    }

    public void StartEpisode()
    {
        int seed = baseSeed + episodeCounter * 10007 + Mathf.RoundToInt(worldOffset.x * 10f);

        var levelGenerator = new LevelGenerator(seed);
        var genParams = new GenerationParams
        {
            chunksToPlace = chunksToPlace,
            startCell = startCell,
        };

        List<PlacedPlaceableData> layout = levelGenerator.Generate(chunkDatabase, genParams);

        levelSpawner.Clear(levelRoot);
        levelSpawner.SpawnLevelFromListLocal(layout, levelRoot);

        Vector2 spawnLocal = PlayerSpawner.FindSpawnLocal(layout);
        spawnLocal.x += 0.5f;
        spawnLocal.y += 0.5f;
        agent.ResetForNewEpisode(spawnLocal);
        agent.arena = this;
        episodeCounter++;
    }
}
