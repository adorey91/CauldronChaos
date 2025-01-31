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

    private Coroutine dayStartCoroutine;

    [Header("MainMenu Selection Animations")]
    [SerializeField] private GameObject menuCamera;
    [SerializeField] private GameObject gameCamera;
 
    private void OnEnable()
    {
        Actions.OnStateChange += UpdateUIForGameState;
        CameraMenuCollider.ReachedWaypoint += CameraReached;
        LevelManager.startTimer += StartDayCountdown;
    }

    private void OnDisable()
    {
        Actions.OnStateChange -= UpdateUIForGameState;
        CameraMenuCollider.ReachedWaypoint -= CameraReached;
        LevelManager.startTimer -= StartDayCountdown;
    }

    private void UpdateUIForGameState(GameManager.GameState state)
    {
        switch (state)
        {
            case GameManager.GameState.MainMenu: MainMenu(); break;
            case GameManager.GameState.Gameplay: Gameplay(); break;
            case GameManager.GameState.LevelSelect: LevelSelect(); break;
            case GameManager.GameState.EndOfDay: EndOfDay(); break;
            case GameManager.GameState.Pause: Pause(); break;
            case GameManager.GameState.Settings: Settings(); break;
        }
    }

    // Toggles the current active canvas
    private void SetActiveUI(Canvas canvas)
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
        MenuVirtualCamera.OnMainMenuCamera?.Invoke();
        SetActiveUI(mainMenu);
        Actions.OnFirstSelect("Menu");
    }

    private void CameraReached(string waypoint)
    {
        if(waypoint == "Door")
        {
            SetActiveUI(intro);
            Actions.OnFirstSelect("Intro");
            return;
        }

        if (waypoint == "Calendar")
        {
            LevelSelect();
        }
    }


    private void LevelSelect()
    {
        SetActiveUI(levelSelect);
        Actions.OnFirstSelect("LevelSelect");
    }


    private void Gameplay()
    {
        MenuVirtualCamera.OnResetCamera?.Invoke();

        SetActiveUI(gameplay);
        Time.timeScale = 1;
    }

    private void EndOfDay()
    {
        SetActiveUI(endOfDay);
        Actions.OnFirstSelect("EndOfDay");
    }

    private void Pause()
    {
        SetActiveUI(pause);
        Actions.OnFirstSelect("Menu");
        Time.timeScale = 0;
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
        dayStartPanel.SetActive(true);
        if (dayStartCoroutine != null)
            StopCoroutine(dayStartCoroutine);
        else
            dayStartCoroutine = StartCoroutine(DayStartTimer());
    }
    private IEnumerator DayStartTimer()
    {
        int timer = secondsToStart; // Initialize timer with total seconds

        while (timer > 0) // Loop until timer reaches -1
        {
            dayStartText.text = $"Day Starts in... {timer}"; // Update text with current timer value
            timer--; // Decrement timer by 1
            yield return new WaitForSeconds(1); // Wait for 1 second
        }

        dayStartPanel.SetActive(false); // Disable the day start panel
        Actions.OnStartDay?.Invoke(); // Invoke the StartDay action
    }
}
