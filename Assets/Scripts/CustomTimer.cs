using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CustomTimer
{
    // duration is measured in seconds
    public float duration;
    public float elapsedTime;
    public bool isRunning;
    public float timeRemaining;

    public CustomTimer(float newDuration, bool isInMinutes)
    {
        // Convert duration to seconds if it's provided in minutes
        duration = isInMinutes ? newDuration * 60 : newDuration;
        elapsedTime = 0;
        isRunning = false;
    }

    public void StartTimer()
    {
        elapsedTime = 0;
        isRunning = true;
    }

    public bool UpdateTimer()
    {
        if (!isRunning) return false;

        elapsedTime += Time.deltaTime;
        if (elapsedTime >= duration)
        {
            isRunning = false;
            return true;
        }
        return false;
    }
    
    public float GetRemainingTime()
    {
        if (!isRunning)
            return 0;

        timeRemaining = Mathf.Max(duration - elapsedTime, 0);
        return timeRemaining;
    }

    public void ResetTimer()
    {
        StartTimer();
    }
}
