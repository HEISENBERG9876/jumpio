using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyTester : MonoBehaviour
{
    public ArenaDifficultyTestEpisodeController arenaController;
    public int tries = 10;
    public float maxSecondsPerTry = 40f;

    public async UniTask<DifficultyReport> TestDifficultyAsync(List<PlacedPlaceableData> layout)
    {
        var report = new DifficultyReport { tries = tries };
        var agent = arenaController.agent;

        Time.timeScale = 20f;

        for (int i = 0; i < tries; i++)
        {
            var taskCompletionSource = new UniTaskCompletionSource<PlayerAgent.EpisodeResult>();

            void Handler(PlayerAgent.EpisodeResult result) => taskCompletionSource.TrySetResult(result);
            agent.EpisodeFinished += Handler;

            await UniTask.Yield();

            PlayerAgent.EpisodeResult result;
            try
            {
                result = await taskCompletionSource.Task.Timeout(TimeSpan.FromSeconds(maxSecondsPerTry));
            }
            catch
            {
                agent.EpisodeFinished -= Handler;
                result = PlayerAgent.EpisodeResult.Timeout;
            }

            agent.EpisodeFinished -= Handler;

            switch (result)
            {
                case PlayerAgent.EpisodeResult.Win: report.wins++; break;
                case PlayerAgent.EpisodeResult.Death: report.deaths++; break;
                case PlayerAgent.EpisodeResult.Timeout: report.timeouts++; break;
            }
        }

        Time.timeScale = 1f;

        report.CalculateDifficulty();
        return report;
    }
}
