using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelCreator : MonoBehaviour
{
    public ToolbarController toolbar;
    public List<PlacedObjectData> currentLayout = new();
    public float cellSize = 1f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlaceObjectAtMouse();
        }
    }

    void AddLevelData(Placeable selection, Vector3 SnappedPos)
    {

        var data = new PlacedObjectData
        {
            id = selection.id,
            x = SnappedPos[0],
            y = SnappedPos[1],
            rotation = 0
        };
        currentLayout.Add(data);

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
        worldPos.z = 0f;

        Vector3 snappedPos = new Vector3(
            Mathf.Floor(worldPos.x / cellSize) * cellSize + cellSize * 0.5f,
            Mathf.Floor(worldPos.y / cellSize) * cellSize + cellSize * 0.5f,
            0f
        );

        snappedPos.y += selection.offsetY;

        Debug.Log($"Placing object at {snappedPos}");

        var go = Instantiate(selection.prefab, snappedPos, Quaternion.identity);

        Debug.Log("Real snapped pos: " + go.transform.position);

        foreach (var toggle in go.GetComponentsInChildren<EditorModeToggle>(true))
        {
            toggle.Apply(true);
        }

        AddLevelData(selection, snappedPos);
    }
}
