using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Difficulty Test Data")]
public class DifficultyTestData : ScriptableObject
{
    public bool hasData;
    public List<PlacedPlaceableData> layout;
    public DifficultyReport report;
    public bool hasReport;

    public event Action<DifficultyReport> ReportReady;

    public void Set(List<PlacedPlaceableData> newLayout)
    {
        hasData = true;
        layout = newLayout;
        report = default;
        hasReport = false;
    }

    public void SetReport(DifficultyReport newReport)
    {
        report = newReport;
        hasReport = true;
        ReportReady?.Invoke(report);
    }

    public void Clear()
    {
        hasData = false;
        layout = null;
        report = default;
    }
}
