using System;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    public event Action TimerEnded;

    private float remainingTime;
    private bool isRunning = false;

    void Update()
    {
        if (!isRunning)
        {
            return;
        }

        remainingTime -= Time.deltaTime;

        if(remainingTime <= 0)
        {
            isRunning = false;
            timerText.text = "00:00";
            TimerEnded?.Invoke();
            StopTimer();
        }
        else
        {
            int minutes = (int) remainingTime / 60;
            int remainingSeconds = (int) remainingTime % 60;
            UpdateCounterText(remainingTime);
        }
    }


    public void StartTimer(int seconds)
    {
        remainingTime = Mathf.Max(0, seconds);
        UpdateCounterText(remainingTime);
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }


    private void UpdateCounterText(float timeSeconds)
    {
        int minutes = Mathf.FloorToInt(timeSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeSeconds % 60f);
        if (timerText != null)
            timerText.text = $"{minutes:D2}:{seconds:D2}";
    }
}
