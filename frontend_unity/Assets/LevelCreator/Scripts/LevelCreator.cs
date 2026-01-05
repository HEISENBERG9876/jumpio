using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlacedRecord
{
    public GameObject gameObject;
    public PlacedObjectData placedObjectData;

    public PlacedRecord(GameObject gameObject, PlacedObjectData placedObjectData)
    {
        this.gameObject = gameObject;
        this.placedObjectData = placedObjectData;
    }
}

//minimum allowable height to place objects should be -11.5
//maximum allowable height should be 9.5
public class LevelCreator : MonoBehaviour
{
    public ToolbarController toolbar;
    public List<PlacedObjectData> currentLayout = new();
    public float cellSize = 1f;
    public bool deleteMode = false;
    private Vector2Int lastActionCell = new Vector2Int(int.MinValue, int.MinValue);
    private Dictionary<Vector2Int, PlacedRecord> placedWithCell = new();

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            deleteMode = !deleteMode;
            Debug.Log($"Delete mode: {deleteMode}");
        }

        if (Input.GetMouseButton(0))
        {
            if(deleteMode)
            {
                DeleteObjectAtMouse();
                return;
            }
            PlaceObjectAtMouse();
        }

        if(Input.GetMouseButtonUp(0))
        {
            lastActionCell = new Vector2Int(int.MinValue, int.MinValue);
        }
    }

    void PlaceObjectAtMouse()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        Placeable selection = toolbar.GetSelection();
        if (selection == null || selection.prefab == null)
        {
            Debug.Log("No placeable selected.");
            return;
        }

        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector2Int cell = WorldToCell(worldPos);
        if (cell == lastActionCell || placedWithCell.ContainsKey(cell))
        {
            return;
        }
        Vector3 snappedPos = CellToWorldCenter(cell);
        snappedPos.y += selection.offsetY;

        Debug.Log($"Placing object at {snappedPos}");

        var go = Instantiate(selection.prefab, snappedPos, Quaternion.identity);

        foreach (var toggle in go.GetComponentsInChildren<EditorModeToggle>(true))
        {
            toggle.Apply(true);
        }

        placedWithCell[cell] = new PlacedRecord(go, new PlacedObjectData
        {
            id = selection.id,
            x = snappedPos.x,
            y = snappedPos.y,
            rotation = 0
        });

        lastActionCell = cell;
        AddLevelData(selection, snappedPos);
    }

    
    void DeleteObjectAtMouse()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;


        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector2Int cell = WorldToCell(worldPos);

        if(cell == lastActionCell)
        {
            return;
        }

        if (placedWithCell.TryGetValue(cell, out PlacedRecord record))
        {
            Destroy(record.gameObject);
            currentLayout.Remove(record.placedObjectData);
            placedWithCell.Remove(cell);
            Debug.Log($"Deleted object at cell {cell}");
        }

        lastActionCell = cell;

    }


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


    void AddLevelData(Placeable selection, Vector3 SnappedPos)
    {

        var data = new PlacedObjectData
        {
            id = selection.id,
            x = SnappedPos.x,
            y = SnappedPos.y,
            rotation = 0
        };
        currentLayout.Add(data);

    }


}