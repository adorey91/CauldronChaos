using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ScoreManager : MonoBehaviour
{
    [Header("Gameplay UI")]
    [SerializeField] private Image quotaFill;
    [SerializeField] private GameObject coinImage;

    [Header("Day Timer")]
    [SerializeField] private int minutesPerDay = 5;
    [SerializeField] private TextMeshProUGUI dayTimerText;
    [SerializeField] private RectTransform timerHand;
    private CustomTimer dayTimer;
    private bool timerStarted;

    [Header("EOD UI")]
    [SerializeField] private TextMeshProUGUI eodTitle;
    [SerializeField] private TextMeshProUGUI eodScoreText;

    [Header("EOD Win Lose Sprite")]
    [SerializeField] private Image eodWinLoseSprite;
    [SerializeField] private Sprite winSprite;
    [SerializeField] private Sprite loseSprite;

    [Header("Score Amounts")]
    [SerializeField] private int tipMultiplier = 2;
    [SerializeField] private int[] scorePerLevel;

    [Header("SFX")]
    [SerializeField] private AudioClip endOfDaySFX;

    // keeps track of current day / day score
    private int score = 0;
    private int currentDay = 0;

    public static Action OnChallengeDay;

    private void Start()
    {
        dayTimer = new CustomTimer(minutesPerDay, true);
        
        quotaFill.fillAmount = 0;

        //dayText.text = $"Day: {currentDay + 1}";
        //scoreText.text = $"Score: {score} / {scorePerLevel[currentDay]}";
        //peopleServedText.text = $"People Served: {people}";
    }

    private void OnEnable()
    {
        Actions.OnCustomerServed += UpdateScore;
        Actions.OnStartDay += StartDay;
        Actions.OnEndDay += UpdateEODText;
        Actions.OnResetValues += ResetValues;
        OnChallengeDay += CheckChallengeDay;
    }

    private void OnDisable()
    {
        Actions.OnCustomerServed -= UpdateScore;
        Actions.OnStartDay -= StartDay;
        Actions.OnEndDay -= UpdateEODText;
        Actions.OnResetValues -= ResetValues;
        OnChallengeDay -= CheckChallengeDay;
    }

    private void Update()
    {
        if (timerStarted)
        {
            SetTimerRotation();

            if (dayTimer.UpdateTimer())
            {
                Actions.OnEndDay?.Invoke();
                Actions.OnForceStateChange("EndOfDay");

                timerStarted = false;

                //playing end of day SFX
                AudioManager.instance.sfxManager.PlaySFX(SFX_Type.ShopSounds, endOfDaySFX, true);
            }
        }
    }

    public void SetCurrentDay(int day)
    {
        currentDay = day;

        //dayText.text = $"Day: {currentDay + 1}";
        //scoreText.text = $"Score: {score} / {scorePerLevel[currentDay]}";
        //peopleServedText.text = $"People Served: {people}";
    }

    private void CheckChallengeDay()
    {
        if ((currentDay + 1) % 2 == 0)
        {
            Debug.Log(currentDay +1);
            ChallengeTrigger.OnStartChallenge?.Invoke();
        }
        else
        {
            if((currentDay +1) > 5)
            {
                GoblinAI.StartGoblinChaos?.Invoke(false);
                Debug.Log("Goblin Chaos");
            }
            Debug.Log("Not a challenge day");
        }
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
            score += addedScore;
            _addToTotalScore = addedScore;
        }
        else
        {
            score += regularScore;
            _addToTotalScore = regularScore;
        }

        quotaFill.fillAmount = (float)score / (float)scorePerLevel[currentDay];
        coinImage.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.5f, 1, 0.5f);
        //scoreText.text = $"Score: {score} / {scorePerLevel[currentDay]}";
        //peopleServedText.text = "People Served: " + people;
    }

    private void UpdateEODText()
    {
        bool increaseDayCount;

        // Check if the player has reached the score for the current day
        if (score < scorePerLevel[currentDay])
            increaseDayCount = false;
        else
            increaseDayCount = true;

        // Save the current day's score and people served
        SaveManager.OnSaveDay(currentDay, score, increaseDayCount);

        // Sets the EOD text
        eodTitle.text = $"End of Day {currentDay + 1}";
        //peopleServedEOD.text = $"People Served: {people}";

        // Sets the EOD win/lose sprite and score text
        if (!increaseDayCount)
        {
            eodScoreText.color = Color.red;
            eodWinLoseSprite.sprite = loseSprite;
            eodScoreText.text = $"Score: {score}\nTry Level Again";
        }
        else
        {
            eodScoreText.color = Color.green;
            eodWinLoseSprite.sprite = winSprite;
            eodScoreText.text = $"Score: {score}";

        }

        score = 0;
    }


    public void ResetValues()
    {
        quotaFill.fillAmount = 0;
        dayTimerText.text = null;
        timerStarted = false;
        timerHand.rotation = Quaternion.Euler(0, 0, 0);
        dayTimer = new CustomTimer(minutesPerDay, true);
        //dayText.text = $"Day: {currentDay}";
        //scoreText.text = $"Score: {score}";
        //peopleServedText.text = $"People Served: {people}";
    }
}
