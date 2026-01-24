using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

//minimum allowable height to place objects should be -11.5
//maximum allowable height should be 9.5
//Width between -86.5 and 86.5
public class LevelCreator : MonoBehaviour
{
    public PlaceableDatabase placeableDatabase;
    public ToolbarController toolbar;
    [SerializeField] Transform creatorRoot;
    public float cellSize = 1f;
    public bool deleteMode = false;
    private Vector2Int lastActionCell = new Vector2Int(int.MinValue, int.MinValue);
    private Dictionary<Vector2Int, PlacedRecord> placedWithCell = new();
    private Stack<ICreatorCommand> undoStack = new();
    private Stack<ICreatorCommand> redoStack = new();
    public float undoRedoMaxTimer = 0.15f;
    private float undoRedoTimer = 0f;
    //generation
    private string entryMarkerId = "EntryMarker"; //better to not hardcode
    private string exitMarkerId = "ExitMarker";
    private Vector2Int generationOriginCell = Vector2Int.zero; //during generation this will be the position where the next chunk will be placed. Also x + 1
    public ChunkDatabase chunkDatabase;
    public TestLevelData testLevelData;
    //testing
    public DifficultyTestData difficultyTestData;
    public TMP_Text difficultyText;

    private void OnEnable()
    {
        difficultyTestData.ReportReady += OnDifficultyReportReady;
    }

    private void OnDisable()
    {
        difficultyTestData.ReportReady -= OnDifficultyReportReady;
    }

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
            {
                return;
            }

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

        var go = Instantiate(placeable.prefab, worldCellCenterPos, Quaternion.identity, creatorRoot);

        foreach (var toggle in go.GetComponentsInChildren<EditorModeToggle>(true))
        {
            toggle.Apply(true);
        }

        PlacedRecord placedRecord = new PlacedRecord(go, new PlacedPlaceableData
        {
            id = placeable.id,
            x = cell.x,
            y = cell.y,
            rotation = 0
        });

        placedWithCell[cell] = placedRecord;
    }


    public void PlaceAtCell(Vector2Int cell, Placeable placeable, PlacedPlaceableData placedData) //placeable parameter not needed
    {
        Vector3 worldCellCenterPos = CellToWorldCenter(cell);
        worldCellCenterPos.y += placeable.offsetY;

        Debug.Log($"Placing object at {worldCellCenterPos}");

        var go = Instantiate(placeable.prefab, worldCellCenterPos, Quaternion.identity, creatorRoot);

        foreach (var toggle in go.GetComponentsInChildren<EditorModeToggle>(true))
        {
            toggle.Apply(true);
        }

        PlacedRecord placedRecord = new PlacedRecord(go, placedData);

        placedWithCell[cell] = placedRecord;
    }


    public Placeable GetPlaceableById(string id)
    {
        foreach (var p in placeableDatabase.placeables)
            if (p != null && p.id == id)
            {
                return p;
            }

        return null;
    }


    public void DeleteAtCell(Vector2Int cell)
    {
        if (placedWithCell.TryGetValue(cell, out PlacedRecord record))
        {
            Destroy(record.gameObject);
            placedWithCell.Remove(cell);
            Debug.Log($"Deleted object at cell {cell}");
        }
    }

    //Undo/Redo
    public void Undo()
    {
        if (undoStack.Count > 0)
        {
            var command = undoStack.Pop();
            command.Undo(this);
            redoStack.Push(command);
        }
    }

    public void Redo()
    {
        if (redoStack.Count > 0)
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
    //cellsize is 1, so dividing/multiplying not really needed
    private Vector2Int WorldToCell(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / cellSize);
        int y = Mathf.FloorToInt(worldPos.y / cellSize);

        return new Vector2Int(x, y);
    }


    public Vector3 CellToWorldCenter(Vector2Int cellPos)
    {
        float x = cellPos.x * cellSize + cellSize * 0.5f;
        float y = cellPos.y * cellSize + cellSize * 0.5f;

        return new Vector3(x, y, 0f);
    }

    //Layout export
    public List<PlacedPlaceableData> GetCurrentLayout()
    {
        List<PlacedPlaceableData> layout = new List<PlacedPlaceableData>();

        foreach (PlacedRecord record in placedWithCell.Values)
        {
            layout.Add(record.placedPlaceableData);
        }

        return layout;
    }

    //generation
    //TODO maybe PlacedPlaceableData and LevelChunk should store Vector2Int instead of x,y for consistency. Also change to int in PlacedPlaceableData and other places with cells
    //chunk width and height probably useless
    
    public void CreateChunkFromPlacablesInScene()
    {
#if UNITY_EDITOR
        if(placedWithCell.Count == 0)
        {
            Debug.LogWarning("Chunks must not be empty");
            return;
        }

        //min and max coordinates used to determine chunk size and make chunk cells start from (0, 0)- useful for later placement in levels
        Vector2Int min = new(int.MaxValue, int.MaxValue);
        Vector2Int max = new(int.MinValue, int.MinValue);

        foreach (Vector2Int cell in placedWithCell.Keys)
        {
            if (cell.x < min.x)
            {
                min.x = cell.x;
            }
            if (cell.y < min.y)
            {
                min.y = cell.y;
            }
            if (cell.x > max.x)
            {
                max.x = cell.x;
            }
            if (cell.y > max.y)
            {
                max.y = cell.y;
            }
        }

        int chunkWidth = max.x - min.x + 1;
        int chunkHeight = max.y - min.y + 1;
        Vector2Int entranceCell = Vector2Int.zero;
        Vector2Int exitCell = Vector2Int.zero;
        int entryMarkerCount = 0; // exactly 1 of entry and exit markers to be valid
        int exitMarkerCount = 0;

        var chunkPlaceables = new List<PlacedPlaceableData>();

        //making cells in chunk use local coordinates starting from (0,0). Also finding entry and exit cells
        foreach (var keyValue in placedWithCell)
        {
            Vector2Int cell = keyValue.Key;
            PlacedPlaceableData data = keyValue.Value.placedPlaceableData;

            int localX = cell.x - min.x;
            int localY = cell.y - min.y;

            if(IsEntryMarker(data))
            {
                entryMarkerCount++;
                entranceCell = new Vector2Int(localX, localY);
                continue;
            }
            if(IsExitMarker(data))
            {
                exitMarkerCount++;
                exitCell = new Vector2Int(localX, localY);
                continue;
            }

            chunkPlaceables.Add(new PlacedPlaceableData
            {
                id = data.id,
                x = localX,
                y = localY,
                rotation = data.rotation
            });
        }

        if(entryMarkerCount != 1 || exitMarkerCount != 1)
        {
            Debug.LogWarning("Chunks must have exactly one entry and one exit marker.");
            return;
        }

        //saving chunk as asset
        string path = UnityEditor.EditorUtility.SaveFilePanelInProject(
            "Save Level  Chunk",
            "NewChunk",
            "asset",
            "Select where to save the chunk"
        );

        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        var chunk = ScriptableObject.CreateInstance<LevelChunk>();
        chunk.placedPlaceablesLocal = chunkPlaceables;
        chunk.entranceCell = entranceCell;
        chunk.exitCell = exitCell;

        UnityEditor.AssetDatabase.CreateAsset(chunk, path);
        UnityEditor.EditorUtility.SetDirty(chunk);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
        Debug.Log("Saved chunk");
#endif
    }

    private bool IsEntryMarker(PlacedPlaceableData data)
    {
        return data.id == entryMarkerId;
    }

    private bool IsExitMarker(PlacedPlaceableData data)
    {
        return data.id == exitMarkerId;
    }

    public void PlaceChunk(LevelChunk chunk, Vector2Int startCell)
    {
        ExecuteCommand(new PlaceChunkCommand(startCell, chunk));
    }

    public void PlaceNextChunk()
    {
        ExecuteCommand(new PlaceChunkCommand(generationOriginCell, GetRandomChunk()));
    }

    public LevelChunk GetRandomChunk()
    {
        return chunkDatabase.chunks[Random.Range(0, chunkDatabase.chunks.Count)];
    }

    public Vector2Int GetGenerationOriginCell()
    {
        return generationOriginCell;
    }

    public void SetGenerationOriginCell(Vector2Int cell)
    {
        generationOriginCell = cell;
    }

    //Testing
    public void OnTestButtonClicked() //manual testing, needs better name
    {
        var currentLayout = GetCurrentLayout();

        if (currentLayout == null || currentLayout.Count == 0)
        {
            Debug.LogWarning("[LevelCreator] Layout cant be empty");
            return;
        }

        testLevelData.Set(currentLayout);

        SceneManager.LoadScene("GameplayScene", LoadSceneMode.Additive);
        CreatorSession.Instance.SetCreatorActive(false);
    }

    public async void OnDifficultyTestButtonClicked()
    {
        var layout = GetCurrentLayout();
        if (layout == null || layout.Count == 0)
        {
            GlobalUIManager.Instance.ShowInfo("Cannot test difficulty on empty layout");
            Debug.LogWarning("[LevelCreator] Layout cant be empty");
            return;
        }

        difficultyTestData.Set(layout);
        GlobalUIManager.Instance.ShowLoading("Evaluating difficulty using bots...");

        await SceneManager.LoadSceneAsync("DifficultyTestScene", LoadSceneMode.Additive);
    }

    private void OnDifficultyReportReady(DifficultyReport report)
    {
        if (difficultyText != null)
        {
            difficultyText.text = report.calculatedDifficulty;
        }
    }


    //validation
    public bool ValidateLayoutForSave()
    {
        var layout = GetCurrentLayout();
        if (layout == null || layout.Count == 0)
        {
            GlobalUIManager.Instance.ShowInfo("Cannot save empty layout");
            return false;
        }

        int spawnCount = 0;
        int finishCount = 0;

        foreach (PlacedPlaceableData p in layout)
        {
            if (p.id == "SpawnMarker")
            {
                spawnCount++;
            }
            if (p.id == "FinishFlag")
            {
                finishCount++;
            }
        }

        if (spawnCount != 1)
        {
            GlobalUIManager.Instance.ShowInfo("Level must contain exactly 1 spawn marker");
            return false;
        }
        if (finishCount < 1)
        {
            GlobalUIManager.Instance.ShowInfo("Level must contain at least 1 finish flag");
            return false;
        }

        return true;
    }
}