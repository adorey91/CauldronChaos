using Cinemachine;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public UiObject currentLocation;

    [Header("Settings")]
    [SerializeField] private SettingsManager settingsManager;

    [Header("Ui Canvas")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gameplay;
    [SerializeField] private GameObject intro;
    [SerializeField] private GameObject levelSelect;
    [SerializeField] private GameObject endOfDay;
    [SerializeField] private GameObject settings;
    [SerializeField] private GameObject pause;
    [SerializeField] private GameObject howToPlay;

    [Header("How To Play")]
    [SerializeField] private Image howToPlayBG;
    [SerializeField] private Button howToPlayBack;


    #region OnEnable / OnDisable / OnDestroy Events
    private void OnEnable()
    {
        Actions.OnStateChange += UpdateUIForGameState;
        Actions.ReachedWaypoint += CameraReached;
        Actions.OnActivateHowToPlay += ActivateHowToPlay;
        Actions.OnDeactivateHowToPlay += DeactivateHowToPlay;
    }

    private void OnDisable()
    {
        Actions.OnActivateHowToPlay -= ActivateHowToPlay;
        Actions.OnDeactivateHowToPlay -= DeactivateHowToPlay;
        Actions.OnStateChange -= UpdateUIForGameState;
        Actions.ReachedWaypoint -= CameraReached;
    }

    private void OnDestroy()
    {
        Actions.OnActivateHowToPlay -= ActivateHowToPlay;
        Actions.OnDeactivateHowToPlay -= DeactivateHowToPlay;
        Actions.OnStateChange -= UpdateUIForGameState;
        Actions.ReachedWaypoint -= CameraReached;
    }
    #endregion

    private void UpdateUIForGameState(GameManager.GameState state)
    {
        //LevelSelect.UpdateLevelButtons?.Invoke();
        Debug.Log("Setting gamestate for " + state);

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

    // Toggles the current active panel
    public void SetActiveUI(GameObject panel)
    {
        Debug.Log("Setting activate panel" + panel);

        mainMenu.SetActive(false);
        intro.SetActive(false);
        levelSelect.SetActive(false);
        gameplay.SetActive(false);
        pause.SetActive(false);
        settings.SetActive(false);
        endOfDay.SetActive(false);
        howToPlay.SetActive(false);

        panel.SetActive(true);
    }


    // State UI Changes.
    #region State_UI_Changes
    private void MainMenu()
    {
        InputManager.Instance.TurnOnInteraction();
        MenuVirtualCamera.TurnCameraBrainOn?.Invoke();
        SetActiveUI(mainMenu);
        Actions.OnSetUiLocation(UiObject.Page.MainMenu);
        Time.timeScale = 1;
    }

    private void CameraReached(string waypoint)
    {
        if (waypoint == "Door")
        {
            SetActiveUI(intro);
            Actions.OnSetUiLocation(UiObject.Page.Intro);
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
        Actions.OnSetUiLocation(UiObject.Page.LevelSelect);
        MenuVirtualCamera.OnResetCamera?.Invoke();
    }


    private void Gameplay()
    {
        MenuVirtualCamera.OnResetCamera?.Invoke();
        SetActiveUI(gameplay);
        Actions.OnSetUiLocation(UiObject.Page.Gameplay);
        Time.timeScale = 1;
    }

    private void EndOfDay()
    {
        SetActiveUI(endOfDay);
        Actions.OnSetUiLocation(UiObject.Page.EndOfDay);
    }

    private void Pause()
    {
        SetActiveUI(pause);
        Actions.OnSetUiLocation(UiObject.Page.Pause);
        Time.timeScale = 0;
    }

    private void Settings()
    {
        SetActiveUI(settings);
        settingsManager.OpenSettings();
    }
    #endregion

    // How to play
    public void ActivateHowToPlay(bool isLoading)
    {
        if (isLoading)
        {
            howToPlay.SetActive(true);
            howToPlayBG.enabled = false;
            howToPlayBack.gameObject.SetActive(false);
        }
        else
        {
            SetActiveUI(howToPlay);
            howToPlayBG.enabled = true;
            howToPlayBack.gameObject.SetActive(true);
        }
    }

    private void DeactivateHowToPlay()
    {
        howToPlay.SetActive(false);
    }
}

[Serializable]
public class UiObject
{
    public enum Page
    {
        MainMenu,
        Intro,
        LevelSelect,
        Pause,
        Gameplay,
        Settings,
        EndOfDay,
        Audio,
        Video,
        System,
        ControlsKeyboard,
        ControlsGamepad,
        DeleteFile,
        HowToPlay,
        DebugInput,
        DebugToggle,
        RecipeBook
    }

    public Page location;
}