using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("EOD Win Lose Sprite")]
    [SerializeField] private Image eodWinLoseSprite;
    [SerializeField] private Sprite winSprite;
    [SerializeField] private Sprite loseSprite;

    [Header("Score Amounts")]
    [SerializeField] private int tipMultiplier = 2;
    [SerializeField] private int[] scorePerLevel = new int[] { 300, 750, 1500, 1700, 1900 };
    //[SerializeField] private int[] scorePerLevel = new int[] { 300, 500, 750, 1000, 1500, 1600, 1700, 1800, 1900, 2000 };

    // keeps track of current day / day score
    private int _score = 0;
    private int _people = 0;
    private int _currentDay = 0;


    private void Start()
    {
        dayTimer = new CustomTimer(minutesPerDay, true);
        

        dayText.text = $"Day: {_currentDay}/{daysToPlay}";
        scoreText.text = $"Score: {_score}";
        peopleServedText.text = $"People Served: {_people}";
    }

    private void OnEnable()
    {
        Actions.OnCustomerServed += UpdateScore;
        Actions.OnStartDay += StartDay;
        Actions.OnEndDay += UpdateEODText;
        Actions.OnResetValues += ResetValues;
    }

    private void OnDisable()
    {
        Actions.OnCustomerServed -= UpdateScore;
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
        _currentDay = day - 1;
    }


    private void StartDay()
    {
        timerStarted = true;
        dayTimer.StartTimer();
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

        scoreText.text = $"Score: {_score} / {scorePerLevel[_currentDay]}";
        peopleServedText.text = "People Served: " + _people;
    }

    private void UpdateEODText()
    {
        bool increaseDayCount;

        // Check if the player has reached the score for the current day
        if (_score < scorePerLevel[_currentDay])
            increaseDayCount = false;
        else
            increaseDayCount = true;

        // Save the current day's score and people served
        SaveManager.OnSaveDay(_currentDay, _score, _people, increaseDayCount);

        // Sets the EOD text
        eodTitle.text = $"End of Day {_currentDay}";
        peopleServedEOD.text = $"People Served: {_people}";

        // Sets the EOD win/lose sprite and score text
        if (!increaseDayCount)
        {
            eodScoreText.color = Color.red;
            eodWinLoseSprite.sprite = loseSprite;
            eodScoreText.text = $"Score: {_score}\nTry Level Again";
        }
        else
        {
            eodScoreText.color = Color.green;
            eodWinLoseSprite.sprite = winSprite;
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
