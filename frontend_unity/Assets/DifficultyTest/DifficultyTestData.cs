using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Difficulty Test Data")]
public class DifficultyTestData : ScriptableObject
{
    public bool hasData;
    public List<PlacedPlaceableData> layout;
    public DifficultyReport report;

    public void Set(List<PlacedPlaceableData> newLayout)
    {
        hasData = true;
        layout = newLayout;
        report = default;
    }

    public void Clear()
    {
        hasData = false;
        layout = null;
        report = default;
    }
}
