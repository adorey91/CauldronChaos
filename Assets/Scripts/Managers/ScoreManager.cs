using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Gameplay UI")]
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI peopleServedText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private int daysToPlay = 5;

    [Header("Day Timer")]
    [SerializeField] private int minutesPerDay = 5;
    [SerializeField] private TextMeshProUGUI dayTimerText;
    [SerializeField] private RectTransform timerHand;
    private CustomTimer dayTimer;
    private bool timerStarted;

    [Header("EOD UI")]
    [SerializeField] private TextMeshProUGUI eodTitle;
    [SerializeField] private TextMeshProUGUI peopleServedEOD;
    [SerializeField] private TextMeshProUGUI eodScoreText;

    [Header("Score Amounts")]
    [SerializeField] private int tipMultiplier = 2;
    [SerializeField] private int noOrderServedScore = -8;

    [Header("Score Needed Per Day")]
    [SerializeField] private int[] scoreDay;

    // keeps track of current day / day score
    private int _score = 0;
    private int _people = 0;
    private int _currentDay;


    private void Start()
    {
        dayTimer = new CustomTimer(minutesPerDay, true);

        _currentDay = 1;
        _score = 0;
        _people = 0;

        dayText.text = $"Day: {_currentDay}/{daysToPlay}";
        scoreText.text = $"Score: {_score}";
        peopleServedText.text = $"People Served: {_people}";
    }

    private void OnEnable()
    {
        Actions.OnCustomerServed += UpdateScore;
        Actions.OnNoCustomerServed += UpdateNoPersonServed;
        Actions.OnStartDay += StartDay;
        Actions.OnEndDay += UpdateEODText;
        Actions.OnResetValues += ResetValues;
    }

    private void OnDisable()
    {
        Actions.OnCustomerServed -= UpdateScore;
        Actions.OnNoCustomerServed -= UpdateNoPersonServed;
        Actions.OnStartDay -= StartDay;
        Actions.OnEndDay -= UpdateEODText;
        Actions.OnResetValues -= ResetValues;
    }

    private void Update()
    {
        //// USED FOR TESTING
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    Actions.OnStartDay?.Invoke();
        //}


        if (timerStarted)
        {
            SetTimerRotation();

            if (dayTimer.UpdateTimer())
            {
                Actions.OnEndDay?.Invoke();
                Actions.OnForceStateChange("EndOfDay");

                timerStarted = false;
            }
        }
    }

    public void SetCurrentDay(int day)
    {
        _currentDay = day;
    }


    private void StartDay()
    {
        timerStarted = true;
        dayTimer.StartTimer();
        Debug.Log("Day Started " + _currentDay);
    }

    private void SetTimerRotation()
    {
        float remainingTime = dayTimer.GetRemainingTime();
        float elapsedTime = dayTimer.elapsedTime;

        // Convert remaining time into minutes and seconds
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);

        dayTimerText.text = $"{minutes:00}:{seconds:00}";

        // Stop any existing rotation animation
        timerHand.DOKill();

        // Calculate how far along the rotation should be (0° to -360°)
        float rotationAngle = Mathf.Lerp(0, -360, elapsedTime / (minutesPerDay * 60));

        // Apply rotation directly without animation
        timerHand.rotation = Quaternion.Euler(0, 0, rotationAngle);
    }



    private void UpdateScore(bool wasGivenOnTime, int regularScore)
    {
        int _addToTotalScore;
        if (wasGivenOnTime)
        {
            int addedScore = regularScore * tipMultiplier;
            _score += addedScore;
            _addToTotalScore = addedScore;
        }
        else
        {
            _score += regularScore;
            _addToTotalScore = regularScore;
        }

        _people++;

        scoreText.text = $"Score: {_score} / {scoreDay[_currentDay]}";
        peopleServedText.text = "People Served: " + _people;
    }

    private void UpdateNoPersonServed()
    {
        _score += noOrderServedScore;
        scoreText.text = "Score: " + _score;
    }

    private void UpdateEODText()
    {
        bool increaseDayCount;

        if (_score < scoreDay[_currentDay])
        {
            increaseDayCount = false;
        }
        else
        {
            increaseDayCount = true;
        }
        SaveManager.SaveInfo(_currentDay, _score, _people, increaseDayCount);

        eodTitle.text = $"End of Day {_currentDay}";
        peopleServedEOD.text = $"People Served: {_people}";

        if (!increaseDayCount)
        {
            eodScoreText.color = Color.red;
            eodScoreText.text = $"Score: {_score}\nTry Level Again";
        }
        else
        {
            eodScoreText.color = Color.green;
            eodScoreText.text = $"Score: {_score}";
        }


        _people = 0;
        _score = 0;
    }

    public void ResetValues()
    {
        Debug.Log("Resetting Time");
        dayTimerText.text = null;
        timerStarted = false;
        timerHand.rotation = Quaternion.Euler(0, 0, 0);
        dayTimer = new CustomTimer(minutesPerDay, true);
        dayText.text = $"Day: {_currentDay}/{daysToPlay}";
        scoreText.text = $"Score: {_score}";
        peopleServedText.text = $"People Served: {_people}";
    }
}
