using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultyTestManager : MonoBehaviour
{
    public DifficultyTestData testData;
    public DifficultyTester difficultyTester;

    private async void Start()
    {
        if (testData == null || !testData.hasData || testData.layout == null || testData.layout.Count == 0)
        {
            Debug.LogError("[DifficultyTestManager] Error with data in test data");
            return;
        }

        if (difficultyTester == null)
        {
            Debug.LogError("[DifficultyTestManager] difficultyTester is null.");
            return;
        }

        testData.report = await difficultyTester.TestDifficultyAsync(testData.layout);

        Debug.Log(
            $"[DifficultyTestManager] Difficulty Test Complete. " +
            $"Calculated Difficulty: {testData.report.calculatedDifficulty} ({testData.report.calculatedDifficulty01:F2}) " +
            $"Win rate: {testData.report.winRate}, Timeout rate: {testData.report.timeoutRate}, death rate: {testData.report.deathRate}"
        );

        await SceneManager.UnloadSceneAsync("DifficultyTestScene");

    }
}
