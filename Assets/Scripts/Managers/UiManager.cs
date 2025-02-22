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

    [Header("SFX")]
    [SerializeField] private AudioClip dayStartSFX;

    private void OnEnable()
    {
        Actions.OnStateChange += UpdateUIForGameState;
        Actions.ReachedWaypoint += CameraReached;
        Actions.SetCursorVisibility += SetCursor;
    }

    private void OnDisable()
    {
        Actions.SetCursorVisibility -= SetCursor;
        Actions.OnStateChange -= UpdateUIForGameState;
        Actions.ReachedWaypoint -= CameraReached;
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
        InputManager.instance.TurnOnInteraction();
        MenuVirtualCamera.TurnCameraBrainOn?.Invoke();
        SetActiveUI(mainMenu);
        Actions.OnFirstSelect("Menu");
        Time.timeScale = 1;
    }

    private void CameraReached(string waypoint)
    {
        if (waypoint == "Door")
        {
            SetActiveUI(intro);
            Actions.OnFirstSelect("Intro");
            return;
        }

        if (waypoint == "Calendar")
        {
            SelectLevel();
        }
    }


    private void SelectLevel()
    {
        Actions.UpdateLevelButtons();
        Time.timeScale = 1;

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
    }

    private void EndOfDay()
    {
        SetActiveUI(endOfDay);
        Actions.OnFirstSelect("EndOfDay");
    }

    private void Pause()
    {
        SetActiveUI(pause);
        Actions.OnFirstSelect("Pause");
        Time.timeScale = 0;
    }

    private void Settings()
    {
        SetActiveUI(settings);
        settingsManager.OpenSettings();
    }
    #endregion


    private void SetCursor(bool isVisible)
    {
        if (isVisible)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
