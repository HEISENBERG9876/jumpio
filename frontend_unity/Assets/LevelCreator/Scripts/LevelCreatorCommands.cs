using UnityEngine;

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
    //private PlacedRecord placedRecord;
    public PlaceCommand(Vector2Int cell, Placeable placeable)
    {
        this.cell = cell;
        this.placeable = placeable;
    }
    public void Execute(LevelCreator levelCreator)
    {
        levelCreator.PlaceAtCell(cell, placeable);
    }
    public void Undo(LevelCreator levelCreator)
    {
        levelCreator.DeleteAtCell(cell);
    }
}

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
