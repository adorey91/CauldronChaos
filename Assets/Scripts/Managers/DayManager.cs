using TMPro;
using UnityEngine;
using System;
using UnityEngine.UI;
using DG.Tweening;

public class DayManager : MonoBehaviour
{
    [Header("Day Timer")]
    [Tooltip("In Minutes")]
    [SerializeField] private float dayLength = 3f;
    [SerializeField] private RectTransform timerHand;
    [SerializeField] private Image timerFill;
    private CustomTimer gameplayTimer;
    private bool gameplayTimerStarted = false;
    private float lastPulseTime = -1f; // Track last pulse time

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
    public static Action<int> OnSetDay;


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
        OnSetDay += SetDay;
    }

    private void OnDisable()
    {
        Actions.OnResetValues -= ResetValues;
        dayText -= SetDayText;
        OnStartDayCountdown -= StartDayCountdown;
        OnSetDay -= SetDay;
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

            if (gameplayTimer.UpdateTimer())
            {
                Actions.OnEndDay?.Invoke();
                Actions.OnForceStateChange("EndOfDay");

                gameplayTimerStarted = false;

                //playing end of day SFX
                AudioManager.instance.sfxManager.PlaySFX(SFX_Type.ShopSounds, endDaySFX, true);
            }
            SetTimerRotation();
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

        timerFill.fillAmount =elapsedTime / (dayLength * 60) ;

        //// Convert remaining time into minutes and seconds
        //int minutes = Mathf.FloorToInt(remainingTime / 60);
        //int seconds = Mathf.FloorToInt(remainingTime % 60);

        //dayTimerText.text = $"{minutes:00}:{seconds:00}";

        // Calculate how far along the rotation should be (0° to -360°)
        float rotationAngle = Mathf.Lerp(0, -360, elapsedTime / (dayLength * 60));

        // Apply rotation directly without animation
        timerHand.rotation = Quaternion.Euler(0, 0, rotationAngle);
        PulseHand();

    }

    private void PulseHand()
    {
        int remainingSeconds = Mathf.FloorToInt(gameplayTimer.GetRemainingTime());
        if (remainingSeconds < 10) return;

        // Check if it's time to pulse and ensure it doesn't repeat in the same second
        if (remainingSeconds <= 60 && remainingSeconds % 15 == 0 && lastPulseTime != remainingSeconds)
        {
            lastPulseTime = remainingSeconds; // Update last pulse time
            timerHand.DOScale(1.2f, 0.2f).SetLoops(2, LoopType.Yoyo);
        }
    }

    private void StartDayCountdown()
    {
        ChallengeManager.CheckChallengeDay?.Invoke();
        ControllerSelect.OnFirstSelect?.Invoke("Gameplay");

        dayStartOverlay.SetActive(true);
        dayCountdownTimer = new(secondsToStart, false);
        dayCountdownTimer.StartTimer();
        dayCountingDown = true;
    }


    private void ResetValues()
    {
        timerFill.fillAmount = 0;
        gameplayTimerStarted = false;
        gameplayTimer.ResetTimer();
        dayCountingDown = false;
        dayCountdownTimer.ResetTimer();
    }
}
