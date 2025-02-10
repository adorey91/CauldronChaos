using Cinemachine;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UiManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private SettingsManager settingsManager;

    [Header("Ui Canvas")]
    [SerializeField] private Canvas mainMenu;
    [SerializeField] private Canvas gameplay;
    [SerializeField] private Canvas intro;
    [SerializeField] private Canvas levelSelect;
    [SerializeField] private Canvas endOfDay;
    [SerializeField] private Canvas settings;
    [SerializeField] private Canvas pause;

    [Header("Day Start Overlay")]
    [SerializeField] private GameObject dayStartPanel;
    [SerializeField] private TextMeshProUGUI dayStartText;
    [SerializeField] private int secondsToStart;
    private CustomTimer dayTimer;
    private bool timerStarted = false;

    private ScoreManager scoreManager;


    private void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();

        if (Gamepad.current != null)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.Confined;

        dayTimer = new(secondsToStart, false);
    }

    private void Update()
    {
        if (timerStarted)
        {
            if (dayTimer.UpdateTimer())
            {
                dayStartPanel.SetActive(false); // Disable the day start panel
                dayTimer.ResetTimer();
                Actions.OnStartDay?.Invoke(); // Invoke the StartDay action
                timerStarted = false;
            }
            else
            {
                int remaining = Mathf.FloorToInt(dayTimer.GetRemainingTime());
                dayStartText.text = $"Day Starts In:\n{remaining}";
            }
        }
    }

    private void OnEnable()
    {
        Actions.OnStateChange += UpdateUIForGameState;
        CameraMenuCollider.ReachedWaypoint += CameraReached;
        LevelManager.startTimer += StartDayCountdown;
        Actions.OnResetValues += ResetTimer;
    }

    private void OnDisable()
    {
        Actions.OnStateChange -= UpdateUIForGameState;
        CameraMenuCollider.ReachedWaypoint -= CameraReached;
        LevelManager.startTimer -= StartDayCountdown;
        Actions.OnResetValues -= ResetTimer;
    }

    private void UpdateUIForGameState(GameManager.GameState state)
    {
        //LevelSelect.UpdateLevelButtons?.Invoke();

        switch (state)
        {
            case GameManager.GameState.MainMenu: MainMenu(); break;
            case GameManager.GameState.Gameplay: Gameplay(); break;
            case GameManager.GameState.LevelSelect: SelectLevel(); break;
            case GameManager.GameState.EndOfDay: EndOfDay(); break;
            case GameManager.GameState.Pause: Pause(); break;
            case GameManager.GameState.Settings: Settings(); break;
        }
    }

    // Toggles the current active canvas
    public void SetActiveUI(Canvas canvas)
    {
        mainMenu.enabled = false;
        intro.enabled = false;
        levelSelect.enabled = false;
        gameplay.enabled = false;
        pause.enabled = false;
        settings.enabled = false;
        endOfDay.enabled = false;

        canvas.enabled = true;
    }


    // State UI Changes.
    #region State_UI_Changes
    private void MainMenu()
    {
        MenuVirtualCamera.TurnCameraBrainOn?.Invoke();
        SetActiveUI(mainMenu);
        Actions.OnFirstSelect("Menu");
    }

    private void CameraReached(string waypoint)
    {
        if (waypoint == "Door")
        {
            SetActiveUI(intro);
            Actions.OnFirstSelect("Intro");
            scoreManager.SetCurrentDay(1);
            return;
        }

        if (waypoint == "Calendar")
        {
            SelectLevel();
        }
    }


    private void SelectLevel()
    {
        LevelSelect.UpdateLevelButtons();
        //if (Gamepad.current != null)
        //    Cursor.lockState = CursorLockMode.Locked;
        //else
            Cursor.lockState = CursorLockMode.Confined;

        Actions.OnResetValues?.Invoke();
        SetActiveUI(levelSelect);
        Actions.OnFirstSelect("LevelSelect");
        MenuVirtualCamera.OnResetCamera?.Invoke();
    }


    private void Gameplay()
    {
        MenuVirtualCamera.OnResetCamera?.Invoke();
        Actions.OnFirstSelect("Gameplay");
        SetActiveUI(gameplay);
        Time.timeScale = 1;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void EndOfDay()
    {
        SetActiveUI(endOfDay);
        Actions.OnFirstSelect("EndOfDay");

        //if (Gamepad.current != null)
        //    Cursor.lockState = CursorLockMode.Locked;
        //else
            Cursor.lockState = CursorLockMode.Confined;
    }

    private void Pause()
    {
        SetActiveUI(pause);
        Actions.OnFirstSelect("Pause");
        Time.timeScale = 0;

        //if (Gamepad.current != null)
        //    Cursor.lockState = CursorLockMode.Locked;
        //else
            Cursor.lockState = CursorLockMode.Confined;
    }

    private void Settings()
    {
        SetActiveUI(settings);
        settingsManager.OpenSettings();

    }
    #endregion


    private void StartDayCountdown()
    {
        Debug.Log("Start Day Countdown");

        Actions.OnFirstSelect("Gameplay");
        dayStartPanel.SetActive(true);
        dayTimer.StartTimer();

        timerStarted = true;
    }

    private void ResetTimer()
    {
        dayTimer.ResetTimer();
        timerStarted = false;
    }
}
