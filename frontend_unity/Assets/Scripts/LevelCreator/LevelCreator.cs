using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    public ToolbarController toolbar;
    public float cellSize = 1f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlaceObjectAtMouse();
        }
    }

    void PlaceObjectAtMouse()
    {
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
    }
}
