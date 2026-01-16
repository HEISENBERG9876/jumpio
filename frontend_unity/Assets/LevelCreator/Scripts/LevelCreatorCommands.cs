using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PlacedRecord
{
    public GameObject gameObject;
    public PlacedPlaceableData placedPlaceableData;

    public PlacedRecord(GameObject gameObject, PlacedPlaceableData placedObjectData)
    {
        this.gameObject = gameObject;
        this.placedPlaceableData = placedObjectData;
    }
}

public interface ICreatorCommand
{
    void Execute(LevelCreator levelCreator);
    void Undo(LevelCreator levelCreator);
}

public class PlaceCommand : ICreatorCommand
{
    private Vector2Int cell;
    private Placeable placeable;

    private Vector2Int originBefore;
    private Vector2Int originAfter;

    //private PlacedRecord placedRecord;
    public PlaceCommand(Vector2Int cell, Placeable placeable)
    {
        this.cell = cell;
        this.placeable = placeable;
    }
    public void Execute(LevelCreator levelCreator)
    {
        originBefore = levelCreator.GetGenerationOriginCell();

        levelCreator.PlaceAtCell(cell, placeable);

        originAfter = cell + Vector2Int.right + Vector2Int.up;
        levelCreator.SetGenerationOriginCell(originAfter);
    }
    public void Undo(LevelCreator levelCreator)
    {
        levelCreator.DeleteAtCell(cell);
        levelCreator.SetGenerationOriginCell(originBefore);
    }
}


//TODO generation origin is not restored properly with delete- not a big issue, since we can change origin manually by placing an object
public class DeleteCommand : ICreatorCommand
{
    private Vector2Int cell;
    private Placeable deletedPlaceable;
    private PlacedRecord deletedRecord;
    public DeleteCommand(Vector2Int cell)
    {
        this.cell = cell;
    }

    public void Execute(LevelCreator levelCreator)
    {
        deletedRecord = levelCreator.GetRecord(cell);

        if (deletedRecord == null)
        {
            return;
        }

        deletedPlaceable = levelCreator.GetPlaceableById(deletedRecord.placedPlaceableData.id);
        levelCreator.DeleteAtCell(cell);
    }

    public void Undo(LevelCreator levelCreator)
    {
        if (deletedRecord == null)
        {
            return;
        }
        PlacedPlaceableData deletedData = deletedRecord.placedPlaceableData;
        levelCreator.PlaceAtCell(cell, deletedPlaceable, deletedData);
    }
}


public class PlaceChunkCommand : ICreatorCommand
{
    private Vector2Int startCell;
    private LevelChunk levelChunk;
    private List<Vector2Int> placedCells = new();
    private Vector2Int originCellBefore;
    private Vector2Int originCellAfter;
    public PlaceChunkCommand(Vector2Int startCell, LevelChunk levelChunk)
    {
        this.startCell = startCell;
        this.levelChunk = levelChunk;
    }

    //TODO check for occupied cells before placing
    public void Execute(LevelCreator levelCreator)
    {
        originCellBefore = levelCreator.GetGenerationOriginCell();

        foreach (PlacedPlaceableData placeableToPlace in levelChunk.placedPlaceablesLocal)
        {
            int worldX = startCell.x + (int)placeableToPlace.x - levelChunk.entranceCell.x; //TODO also remove conversion to int
            int worldY = startCell.y + (int)placeableToPlace.y - levelChunk.entranceCell.y; //removing entranceCell offset to treat entranceCell as (0, 0)
            Vector2Int worldCell = new(worldX, worldY);
            Placeable placeable = levelCreator.GetPlaceableById(placeableToPlace.id);

            PlacedPlaceableData worldPlaceableData = new PlacedPlaceableData
            {
                id = placeableToPlace.id,
                x = worldX,
                y = worldY,
                rotation = placeableToPlace.rotation
            };

            levelCreator.PlaceAtCell(worldCell, placeable, worldPlaceableData);
            placedCells.Add(worldCell);
        }

        originCellAfter = new(startCell.x + levelChunk.exitCell.x - levelChunk.entranceCell.x + 1, startCell.y + levelChunk.exitCell.y - levelChunk.entranceCell.y);
        levelCreator.SetGenerationOriginCell(originCellAfter);
    }
    public void Undo(LevelCreator levelCreator)
    {
        for(int i = placedCells.Count - 1; i >= 0; i--)
        {
            levelCreator.DeleteAtCell(placedCells[i]);
        }

        levelCreator.SetGenerationOriginCell(originCellBefore);
    }
}
