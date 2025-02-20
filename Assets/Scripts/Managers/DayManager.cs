using TMPro;
using UnityEngine;
using System;

public class DayManager : MonoBehaviour
{
    [Header("Day Timer")]
    [Tooltip("In Minutes")]
    [SerializeField] private float dayLength = 3f;
    [SerializeField] private RectTransform timerHand;
    private CustomTimer gameplayTimer;
    private bool gameplayTimerStarted = false;

    private int currentDay;

    [Header("Day Start Overlay")]
    [SerializeField] private GameObject dayStartOverlay;
    [SerializeField] private TextMeshProUGUI dayStartText;
    [SerializeField] private TextMeshProUGUI dayStartExplain;
    [SerializeField] private float secondsToStart = 4f;
    private CustomTimer dayCountdownTimer;
    private bool dayCountingDown = false;

    [Header("SFX")]
    [SerializeField] private AudioClip startDaySFX;
    [SerializeField] private AudioClip endDaySFX;

    public static Action<string> dayText;
    public static Action OnStartDayCountdown;


    // Start is called before the first frame update
    void Start()
    {
        gameplayTimer = new CustomTimer(dayLength, true);
        dayCountdownTimer = new CustomTimer(secondsToStart, false);
    }

    private void OnEnable()
    {
        Actions.OnResetValues += ResetValues;
        dayText += SetDayText;
        OnStartDayCountdown += StartDayCountdown;
    }

    private void OnDisable()
    {
        Actions.OnResetValues -= ResetValues;
        dayText -= SetDayText;
        OnStartDayCountdown -= StartDayCountdown;
    }

    private void Update()
    {
        if(dayCountingDown)
        {
            if (dayCountdownTimer.UpdateTimer())
            {
                AudioManager.instance.sfxManager.PlaySFX(SFX_Type.ShopSounds, startDaySFX, true);
                dayStartOverlay.SetActive(false);
                dayCountingDown = false;
                Actions.OnStartDay?.Invoke();
                StartDay();
            }
            int remaining = Mathf.FloorToInt(dayCountdownTimer.GetRemainingTime());
            dayStartText.text = $"Day {currentDay} Starts In:\n{remaining}";
        }

        if (gameplayTimerStarted)
        {
            SetTimerRotation();

            if (gameplayTimer.UpdateTimer())
            {
                Actions.OnEndDay?.Invoke();
                Actions.OnForceStateChange("EndOfDay");

                gameplayTimerStarted = false;

                //playing end of day SFX
                AudioManager.instance.sfxManager.PlaySFX(SFX_Type.ShopSounds, endDaySFX, true);
            }
        }
    }

    private void StartDay()
    {
        gameplayTimerStarted = true;
        gameplayTimer.StartTimer();
    }

    private void SetDayText(string text)
    {
        dayStartExplain.text = text;
    }

    public void SetDay(int day)
    {
        currentDay = day;
    }

    private void SetTimerRotation()
    {
        float remainingTime = gameplayTimer.GetRemainingTime();
        float elapsedTime = gameplayTimer.elapsedTime;

        //// Convert remaining time into minutes and seconds
        //int minutes = Mathf.FloorToInt(remainingTime / 60);
        //int seconds = Mathf.FloorToInt(remainingTime % 60);

        //dayTimerText.text = $"{minutes:00}:{seconds:00}";

        // Stop any existing rotation animation
        //timerHand.DOKill();

        // Calculate how far along the rotation should be (0° to -360°)
        float rotationAngle = Mathf.Lerp(0, -360, elapsedTime / (dayLength * 60));

        // Apply rotation directly without animation
        timerHand.rotation = Quaternion.Euler(0, 0, rotationAngle);
    }

    private void StartDayCountdown()
    {
        Debug.Log("Starting Day Countdown");
        ChallengeManager.CheckChallengeDay?.Invoke();
        ControllerSelect.OnFirstSelect?.Invoke("Gameplay");

        dayStartOverlay.SetActive(true);
        dayCountdownTimer = new(secondsToStart, false);
        dayCountdownTimer.StartTimer();
        dayCountingDown = true;
    }


    private void ResetValues()
    {
        gameplayTimerStarted = false;
        gameplayTimer.ResetTimer();
        dayCountingDown = false;
        dayCountdownTimer.ResetTimer();
    }
}
