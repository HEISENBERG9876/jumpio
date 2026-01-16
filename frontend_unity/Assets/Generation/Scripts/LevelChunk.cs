using System.Collections.Generic;
using UnityEngine;

public enum ChunkRole { Start, Middle, End }

public enum ChunkTag
{
    Flat,
    Gap,

    Spikes,
    GroundEnemy,
    FlyingEnemy,
    RangedEnemy,

    MovingPlatforms
}

public enum ChunkDifficulty
{
    Easy,
    Medium,
    Hard
}

[CreateAssetMenu(menuName = "Scriptable Objects/LevelChunk")]
public class LevelChunk : ScriptableObject
{
    public Vector2Int entranceCell;
    public Vector2Int exitCell;

    public ChunkRole role;
    [Range(0, 2)]
    public ChunkDifficulty difficulty;
    public List<ChunkTag> tags;

    public List<PlacedPlaceableData> placedPlaceablesLocal = new();
}