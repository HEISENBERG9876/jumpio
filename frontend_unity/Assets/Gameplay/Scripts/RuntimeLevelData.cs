using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public enum RuntimeLevelMode { Play, Edit, CreateNew }

[CreateAssetMenu(fileName = "RuntimeLevelData", menuName = "Scriptable Objects/RuntimeLevelData")]
public class RuntimeLevelData : ScriptableObject
{
    public int levelId = -1;
    public RuntimeLevelMode mode = RuntimeLevelMode.CreateNew;
    public string title;
    public string difficulty;
    public int timer;
    public string layoutUrl;
    [HideInInspector] public List<PlacedPlaceableData> cachedLayout;

    public void Clear()
    {
        levelId = -1;
        mode = RuntimeLevelMode.CreateNew;
        title = "";
        difficulty = "";
        timer = 0;
        layoutUrl = "";
        cachedLayout?.Clear();
    }
}
