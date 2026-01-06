using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//minimum allowable height to place objects should be -11.5
//maximum allowable height should be 9.5
//Width between -86.5 and 86.5
//TODO current layout is redundant
public class LevelCreator : MonoBehaviour
{
    public PlaceableDatabase placeableDatabase;
    public ToolbarController toolbar;
    public List<PlacedObjectData> currentLayout = new();
    public float cellSize = 1f;
    public bool deleteMode = false;
    private Vector2Int lastActionCell = new Vector2Int(int.MinValue, int.MinValue);
    private Dictionary<Vector2Int, PlacedRecord> placedWithCell = new();
    private Stack<ICreatorCommand> undoStack = new();
    private Stack<ICreatorCommand> redoStack = new();
    public float undoRedoMaxTimer = 0.15f;
    private float undoRedoTimer = 0f;

    void Update()
    {
        undoRedoTimer += Time.deltaTime;

        if (Input.GetKey(KeyCode.Z))
        {
            if (undoRedoTimer >= undoRedoMaxTimer)
            {
                Undo();
                undoRedoTimer = 0f;
            }
        }
        else if (Input.GetKey(KeyCode.Y))
        {
            if (undoRedoTimer >= undoRedoMaxTimer)
            {
                Redo();
                undoRedoTimer = 0f;
            }
        }
        else
        {
            undoRedoTimer = undoRedoMaxTimer;
        }


        if (Input.GetKeyDown(KeyCode.X))
        {
            deleteMode = !deleteMode;
            Debug.Log($"Delete mode: {deleteMode}");
        }

        if (Input.GetMouseButton(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            Vector3 mousePos = Input.mousePosition;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            Vector2Int cell = WorldToCell(worldPos);

            if (cell == lastActionCell)
            {
                return;
            }

            lastActionCell = cell;

            if (deleteMode)
            {
                TryDeleteCommand(cell);
                return;
            }

            TryPlaceCommand(cell);
        }

        if (Input.GetMouseButtonUp(0))
        {
            lastActionCell = new Vector2Int(int.MinValue, int.MinValue);
        }
    }

    private void TryPlaceCommand(Vector2Int cell)
    {
        Placeable selectedPlaceable = toolbar.GetSelection();

        if (selectedPlaceable == null || selectedPlaceable.prefab == null)
        {
            Debug.Log("No placeable selected.");
            return;
        }

        if (placedWithCell.ContainsKey(cell))
        {
            return;
        }

        ExecuteCommand(new PlaceCommand(cell, selectedPlaceable));
    }

    private void TryDeleteCommand(Vector2Int cell)
    {
        if (!placedWithCell.ContainsKey(cell))
        {
            return;
        }

        ExecuteCommand(new DeleteCommand(cell));
    }


    public void PlaceAtCell(Vector2Int cell, Placeable placeable)
    {
        Vector3 worldCellCenterPos = CellToWorldCenter(cell);
        worldCellCenterPos.y += placeable.offsetY;

        Debug.Log($"Placing object at {worldCellCenterPos}");

        var go = Instantiate(placeable.prefab, worldCellCenterPos, Quaternion.identity);

        foreach (var toggle in go.GetComponentsInChildren<EditorModeToggle>(true))
        {
            toggle.Apply(true);
        }

        PlacedRecord placedRecord = new PlacedRecord(go, new PlacedObjectData
        {
            id = placeable.id,
            x = worldCellCenterPos.x,
            y = worldCellCenterPos.y,
            rotation = 0
        });

        placedWithCell[cell] = placedRecord;
        currentLayout.Add(placedWithCell[cell].placedObjectData);
    }


    public void PlaceAtCell(Vector2Int cell, Placeable placeable, PlacedObjectData placedData)
    {
        Vector3 worldCellCenterPos = CellToWorldCenter(cell);
        worldCellCenterPos.y += placeable.offsetY;

        Debug.Log($"Placing object at {worldCellCenterPos}");

        var go = Instantiate(placeable.prefab, worldCellCenterPos, Quaternion.identity);

        foreach (var toggle in go.GetComponentsInChildren<EditorModeToggle>(true))
        {
            toggle.Apply(true);
        }

        PlacedRecord placedRecord = new PlacedRecord(go, placedData);

        placedWithCell[cell] = placedRecord;
        currentLayout.Add(placedWithCell[cell].placedObjectData);
    }


    public Placeable GetPlaceableById(string id)
    {
        foreach (var p in placeableDatabase.placeables)
            if (p != null && p.id == id)
                return p;

        return null;
    }


    public void DeleteAtCell(Vector2Int cell)
    {
        if (placedWithCell.TryGetValue(cell, out PlacedRecord record))
        {
            Destroy(record.gameObject);
            currentLayout.Remove(record.placedObjectData);
            placedWithCell.Remove(cell);
            Debug.Log($"Deleted object at cell {cell}");
        }
    }

    //Undo/Redo
    public void Undo()
    {
        if(undoStack.Count > 0)
        {
            var command = undoStack.Pop();
            command.Undo(this);
            redoStack.Push(command);
        }
    }

    public void Redo()
    {
        if(redoStack.Count > 0)
        {
            var command = redoStack.Pop();
            command.Execute(this);
            undoStack.Push(command);
        }
    }

    private void ExecuteCommand(ICreatorCommand command)
    {
        command.Execute(this);
        undoStack.Push(command);
        redoStack.Clear();
    }

    public PlacedRecord GetRecord(Vector2Int cell)
    {
        if (placedWithCell.TryGetValue(cell, out PlacedRecord record))
        {
            return record;
        }
        return null;
    }



    //Grid
    private Vector2Int WorldToCell(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / cellSize);
        int y = Mathf.FloorToInt(worldPos.y / cellSize);
        return new Vector2Int(x, y);
    }


    private Vector3 CellToWorldCenter(Vector2Int cellPos)
    {
        float x = cellPos.x * cellSize + cellSize * 0.5f;
        float y = cellPos.y * cellSize + cellSize * 0.5f;
        return new Vector3(x, y, 0f);
    }
}