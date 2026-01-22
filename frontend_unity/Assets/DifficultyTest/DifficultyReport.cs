using System;
using UnityEngine;

[Serializable]
public struct DifficultyReport
{
    public int tries;
    public int wins;
    public int deaths;
    public int timeouts;

    [Range(0, 1)] public float winRate;
    [Range(0, 1)] public float deathRate;
    [Range(0, 1)] public float timeoutRate;

    public string calculatedDifficulty;
    public float calculatedDifficulty01; // 0 easy, 1 hard

    public void CalculateDifficulty()
    {
        if (tries <= 0)
        {
            winRate = deathRate = timeoutRate = 0f;
            calculatedDifficulty = "Error";
            calculatedDifficulty01 = 0f;
            Debug.Log("tries should be higher than 0");
            return;
        }

        winRate = (float)wins / tries;
        deathRate = (float)deaths / tries;
        timeoutRate = (float)timeouts / tries;


        float score = 0f;
        score += (1f - winRate) * 0.75f;
        score += timeoutRate * 0.20f;
        score += deathRate * 0.05f;
        calculatedDifficulty01 = Mathf.Clamp01(score);

        if (winRate >= 0.70f)
        {
            calculatedDifficulty = "Easy";
        }
        else if (winRate >= 0.30f)
        {
            calculatedDifficulty = "Medium";
        }
        else if (winRate >= 0f)
        {
            calculatedDifficulty = "Hard";
        }
        //todo add very hard/impossible
    }
}
