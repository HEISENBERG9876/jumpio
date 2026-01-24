using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultyTestManager : MonoBehaviour
{
    public DifficultyTestData testData;
    public DifficultyTester difficultyTester;

    private async void Start()
    {
        try
        {
            if (testData == null || !testData.hasData || testData.layout == null || testData.layout.Count == 0)
            {
                GlobalUIManager.Instance.ShowInfo("Layout cannot be empty when testing difficulty.");
                Debug.LogError("[DifficultyTestManager] Error with data in test data");
                return;
            }

            if (difficultyTester == null)
            {
                Debug.LogError("[DifficultyTestManager] difficultyTester is null.");
                return;
            }

            GlobalUIManager.Instance.ShowLoading("Evaluating difficulty using bots...");
            testData.SetReport(await difficultyTester.TestDifficultyAsync(testData.layout));

            Debug.Log(
                $"[DifficultyTestManager] Difficulty Test Complete. " +
                $"Calculated Difficulty: {testData.report.calculatedDifficulty} ({testData.report.calculatedDifficulty01:F2}) " +
                $"Win rate: {testData.report.winRate}, Timeout rate: {testData.report.timeoutRate}, death rate: {testData.report.deathRate}"
            );

            await SceneManager.UnloadSceneAsync("DifficultyTestScene");
        }
        finally
        {
            GlobalUIManager.Instance.ShowInfo("Difficulty evaluation finished.");
            GlobalUIManager.Instance.HideLoading();
        }
        

    }
}
