using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RuntimeLevelData", menuName = "Scriptable Objects/RuntimeLevelData")]
public class RuntimeLevelData : ScriptableObject
{
    public string title;
    public string difficulty;
    public int timer;
    public string layoutUrl;
    [HideInInspector] public List<PlacedObjectData> cachedLayout;

    public void Clear()
    {
        title = "";
        difficulty = "";
        timer = 0;
        layoutUrl = "";
    }
}
