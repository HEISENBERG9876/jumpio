using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GenerationParams
{
    public int chunksToPlace;
    public Vector2Int startCell;
    public int minMiddleChunks = 1;
    public int maxMiddleChunks = 6;
}

public enum DifficultyMode
{
    Exact,
    UpTo,
    UpToMixed
}


public class LevelGenerator
{
    private System.Random rng;

    public LevelGenerator(int seed)
    {
        rng = new System.Random(seed);
    }

    public List<PlacedPlaceableData> Generate(ChunkDatabase chunkDatabase, GenerationParams generationParams)
    {
        if (generationParams.chunksToPlace < 2)
            throw new ArgumentException("chunksToPlace must be at least 2 (Start + End).");

        var starts = new List<LevelChunk>();
        var middles = new List<LevelChunk>();
        var ends = new List<LevelChunk>();

        foreach (LevelChunk levelChunk in chunkDatabase.chunks)
        {
            if (levelChunk == null) continue;
            switch (levelChunk.role)
            {
                case ChunkRole.Start: starts.Add(levelChunk); break;
                case ChunkRole.Middle: middles.Add(levelChunk); break;
                case ChunkRole.End: ends.Add(levelChunk); break;
            }
        }

        if (starts.Count == 0)
        {
            throw new InvalidOperationException("No Start chunks in database.");
        }
        if (middles.Count == 0)
        {
            throw new InvalidOperationException("No Middle chunks in database.");
        }
        if (ends.Count == 0)
        {
            throw new InvalidOperationException("No End chunks in database.");
        }

        List<PlacedPlaceableData> layout = new();
        Vector2Int origin = generationParams.startCell;

        origin = AppendChunk(layout, starts[rng.Next(starts.Count)], origin);

        int middleCount = generationParams.chunksToPlace - 2;
        for (int i = 0; i < middleCount; i++)
        {
            var mid = middles[rng.Next(middles.Count)];
            origin = AppendChunk(layout, mid, origin);
        }

        origin = AppendChunk(layout, ends[rng.Next(ends.Count)], origin);

        return layout;
    }

    //TODO reuse in level creator commands
    private Vector2Int AppendChunk(
       List<PlacedPlaceableData> layout,
       LevelChunk chunk,
       Vector2Int startCell
       )
    {
 

        Vector2Int nextOrigin = new(
            startCell.x + chunk.exitCell.x - chunk.entranceCell.x + 1,
            startCell.y + chunk.exitCell.y - chunk.entranceCell.y
        );

        foreach (PlacedPlaceableData placedPlaceable in chunk.placedPlaceablesLocal)
        {
            int worldX = startCell.x + (int)placedPlaceable.x - chunk.entranceCell.x;
            int worldY = startCell.y + (int)placedPlaceable.y - chunk.entranceCell.y;


            layout.Add(new PlacedPlaceableData
            {
                id = placedPlaceable.id,
                x = worldX,
                y = worldY,
                rotation = placedPlaceable.rotation
            });
        }

        return nextOrigin;
    }


    private static bool DifficultyMatches(ChunkDifficulty chunkDiff, ChunkDifficulty target, DifficultyMode mode)
    {
        return mode switch
        {
            DifficultyMode.Exact => chunkDiff == target,
            DifficultyMode.UpTo => chunkDiff <= target,
            DifficultyMode.UpToMixed => chunkDiff <= target,
            _ => true,
        };
    }

}
