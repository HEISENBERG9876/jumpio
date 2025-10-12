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
            x = (int)SnappedPos[0],
            y = (int)SnappedPos[1],
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
            Mathf.Round(worldPos.x / cellSize) * cellSize,
            Mathf.Round(worldPos.y / cellSize) * cellSize,
            0f
        );

        Instantiate(selection.prefab, snappedPos, Quaternion.identity);
        AddLevelData(selection, snappedPos);

        
    Debug.Log($"Added to layout: {selection.id} at ({snappedPos.x}, {snappedPos.y})");
    }
}
