using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UITimerManager : MonoBehaviour
{
    #region Variables

    [Header("Timer Variables")]
    [SerializeField]
    private float timerDuration = 3f * 60f;
    [SerializeField]
    private bool activateCountDown = true;
    private float timer;

    [Header("Display Timer TextMesh GUI")]
    [SerializeField]
    private TextMeshProUGUI firstMinute;
    [SerializeField]
    private TextMeshProUGUI secondMinute;
    [SerializeField]
    private TextMeshProUGUI seperator;
    [SerializeField]
    private TextMeshProUGUI firstSecond;
    [SerializeField]
    private TextMeshProUGUI secondSecond;

    private float flashTimer;
    private float flashDuration = 1f;

    #endregion

    #region Built In Methods

    private void Start()
    {
        ResetTimer();
    }

    private void Update()
    {
        if (GameManager.CheckPlayerStatus() == false)
            Flash();
        else
            ManageTimer();
    }

    #endregion

    public void ResetTimer()
    {
        if (activateCountDown)
        {
            timer = timerDuration;
        }
        else
        {
            timer = 0;
        }

        SetTextDisplay(true);
    }

    private void ManageTimer()
    {
        if (activateCountDown && timer > 0)
        {
            timer -= Time.deltaTime;
            UpdateTimerDisplay(timer);
        }
        else if (!activateCountDown && timer < timerDuration)
        {
            timer += Time.deltaTime;
            UpdateTimerDisplay(timer);
        }
        else
            Flash();
    }

    private void UpdateTimerDisplay(float timer)
    {
        if (timer < 0)
        {
            timer = 0;
        }

        float minutes = Mathf.FloorToInt(timer / 60);
        float seconds = Mathf.FloorToInt(timer % 60);

        string currentTime = string.Format("{00:00}{1:00}", minutes, seconds);
        firstMinute.text = currentTime[0].ToString();
        secondMinute.text = currentTime[1].ToString();
        firstSecond.text = currentTime[2].ToString();
        secondSecond.text = currentTime[3].ToString();

    }

    private void Flash()
    {
        //if (activateCountDown && timer != 0)
        //{
        //    timer = 0;
        //    UpdateTimerDisplay(timer);
        //}
        //if (!activateCountDown && timer != timerDuration)
        //{
        //    timer = timerDuration;
        //    UpdateTimerDisplay(timer);
        //}

        if (flashTimer <= 0)
        {
            flashTimer = flashDuration;
        }
        else if (flashTimer >= flashDuration / 2)
        {
            flashTimer -= Time.deltaTime;
            SetTextDisplay(false);
        }
        else
        {
            flashTimer -= Time.deltaTime;
            SetTextDisplay(true);
        }
    }

    private void SetTextDisplay(bool enabled)
    {
        firstMinute.enabled = enabled;
        secondMinute.enabled = enabled;
        seperator.enabled = enabled;
        firstSecond.enabled = enabled;
        secondSecond.enabled = enabled;
    }

}
