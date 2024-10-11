using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeStopHandler : MonoBehaviour
{
    public static TimeStopHandler instance { get; private set; }
    public float pauseDuration = 0.1f;  // Default duration of the time stop (can still be used if needed)
    private float timeRemaining;  // Time left for the pause (if using duration-based stop)
    private bool isTimeStopped = false; // Whether time is currently stopped

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        // If time is stopped and has a duration, count down the remaining time
        if (isTimeStopped && timeRemaining > 0f)
        {
            timeRemaining -= Time.unscaledDeltaTime;

            // Resume time once the countdown is over
            if (timeRemaining <= 0f)
            {
                ResumeTime();
            }
        }
    }

    // Method to stop time for impact with a set duration
    public void StopTimeForImpact(float duration = -1f)
    {
        if (!isTimeStopped)
        {
            isTimeStopped = true;
            timeRemaining = duration > 0 ? duration : -1f;  // Use given duration or stop indefinitely if -1
            Time.timeScale = 0f;  // Stop time
        }
    }

    // Method to stop time indefinitely (no duration specified)
    public void StopTimeIndefinitely()
    {
        StopTimeForImpact(-1f);  // Call with no duration to stop indefinitely
    }

    // Method to resume time (can be called externally)
    public void ResumeTime()
    {
        Time.timeScale = 1f;  // Resume time
        isTimeStopped = false;
        timeRemaining = 0f;  // Reset the countdown if applicable
    }
}
