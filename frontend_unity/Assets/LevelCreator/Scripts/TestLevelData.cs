using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TestLevelData", menuName = "Scriptable Objects/TestLevelData")]
public class TestLevelData : ScriptableObject
{
    public int timer;
    public bool hasData;
    [HideInInspector] public List<PlacedPlaceableData> layout;

    public void Set(int timer, List<PlacedPlaceableData> layout)
    {
        this.timer = timer;
        this.layout = new List<PlacedPlaceableData>(layout);
        hasData = true;
    }

    public void Clear()
    {
        timer = 0;
        hasData = false;
        layout?.Clear();
    }
}
