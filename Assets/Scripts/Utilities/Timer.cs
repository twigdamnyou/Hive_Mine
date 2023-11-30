using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Timer
{

    public float Ratio { get { return timeElapsed / Duration; } }
    public float RatioOfRemaining { get { return Mathf.Abs(Ratio - 1f); } }
    public float Duration { get; private set; }

    private float timeElapsed;
    private bool resetTimerOnComplete;
    private Action onCompleteCallBack;


    public Timer(float duration, Action onCompleteCallBack, bool resetTimerOnComplete = true)
    {
        if (duration <= 0)
        {
            Debug.LogError("A timer was given a duation of 0 or less. This timer will  not work");
        }

        Duration = duration;
        this.resetTimerOnComplete = resetTimerOnComplete;
        this.onCompleteCallBack = onCompleteCallBack;
    }

    public void UpdateClock()
    {
        if (timeElapsed < Duration)
        {
            timeElapsed += Time.deltaTime;

            if (timeElapsed >= Duration)
            {
                if (onCompleteCallBack != null)
                    onCompleteCallBack();
                if (resetTimerOnComplete == true)
                {
                    ResetTimer();
                }
            }
        }
    }

    public void ModifyDuration(float ammount)
    {
        Duration += ammount;
        if (Duration <= 0f)
        {
            Duration = 0f;
        }
        if (timeElapsed > Duration)
        {
            timeElapsed = 0f;
        }
    }

    public void ResetTimer()
    {
        timeElapsed = 0f;
    }

}