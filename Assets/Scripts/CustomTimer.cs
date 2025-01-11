using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CustomTimer
{
    public float duration;
    public float elapsedTime;
    public bool isRunning;

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

        return Mathf.Max(duration - elapsedTime, 0);
    }

    public void ResetTimer()
    {
        StartTimer();
    }
}
