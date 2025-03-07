using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public Page currentLocation;

    [Header("Settings")]
    [SerializeField] private SettingsManager settingsManager;

    [Header("Ui Canvas")]
    [SerializeField] private GameObject mainMenuPanelUI;
    [SerializeField] private GameObject gameplayPanelUI;
    [SerializeField] private GameObject introPanelUI;
    [SerializeField] private GameObject levelSelectPanelUI;
    [SerializeField] private GameObject endOfDayPanelUI;
    [SerializeField] private GameObject settingsPanelUI;
    [SerializeField] private GameObject pausePanelUI;
    [SerializeField] private GameObject howToPlayPanelUI;

    [Header("How To Play")]
    [SerializeField] private Image howToPlayBg;
    [SerializeField] private Button howToPlayBack;
    
    [Header("Loading Screen")]
    [SerializeField] private Canvas loadingScreen;
    [SerializeField] private Slider loadingBar;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private float fakeProgressSpeed = 0.2f;
    private Animator _fadeAnimator;
    
    private Dictionary<GameState, (GameObject panel, Action action)> _uiElements;

    // Callback function to be invoked after fade animation completes
    private Action _fadeCallback;
    
    private void Awake()
    {
        // initialize panel dictionary
        _uiElements = new Dictionary<GameState, (GameObject, Action)>
        {
            { GameState.MainMenu, (mainMenuPanelUI, MainMenu) },
            { GameState.Intro, (introPanelUI, Intro) },
            { GameState.LevelSelect, (levelSelectPanelUI, LevelSelect) },
            { GameState.Gameplay, (gameplayPanelUI, Gameplay) },
            { GameState.EndOfDay, (endOfDayPanelUI, EndOfDay) },
            { GameState.Settings, (settingsPanelUI, Settings) },
            { GameState.Pause, (pausePanelUI, Pause) },
        };
    }

    // private void Start()
    // {
    //     _fadeAnimator = GetComponent<Animator>();
    //     _fadeAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
    // }

    #region OnEnable / OnDisable / OnDestroy Events

    private void OnEnable()
    {
        Actions.OnChangeUi += UpdateUIForGameState;
        Actions.ReachedWaypoint += CameraReached;
        Actions.OnActivateHowToPlay += ActivateHowToPlay;
        Actions.OnDeactivateHowToPlay += DeactivateHowToPlay;
    }

    private void OnDisable()
    {
        Actions.OnActivateHowToPlay -= ActivateHowToPlay;
        Actions.OnDeactivateHowToPlay -= DeactivateHowToPlay;
        Actions.OnChangeUi -= UpdateUIForGameState;
        Actions.ReachedWaypoint -= CameraReached;
    }

    private void OnDestroy()
    {
        Actions.OnActivateHowToPlay -= ActivateHowToPlay;
        Actions.OnDeactivateHowToPlay -= DeactivateHowToPlay;
        Actions.OnChangeUi -= UpdateUIForGameState;
        Actions.ReachedWaypoint -= CameraReached;
    }

    #endregion

    private void UpdateUIForGameState(GameState state)
    {
        Debug.Log("Setting gamestate for " + state);

        foreach (var uiElement in _uiElements.Values)
        {
            uiElement.panel.SetActive(false);
        }

        if (!_uiElements.TryGetValue(state, out var element)) return;
        element.panel.SetActive(true);
        element.action?.Invoke();
    }

    // State UI Changes.

    #region State_UI_Changes

    private void MainMenu()
    {
        InputManager.Instance.TurnOnInteraction();
        MenuVirtualCamera.TurnCameraBrainOn?.Invoke();
        Actions.OnSetUiLocation(Page.MainMenu);
        Time.timeScale = 1;
    }

    private void CameraReached(string waypoint)
    {
        switch (waypoint)
        {
            case "Door":
                Actions.OnStateChange("Intro");
                return;
            case "Calendar": Actions.OnStateChange("LevelSelect"); break;
        }
    }


    private void LevelSelect()
    {
        Actions.UpdateLevelButtons();
        Time.timeScale = 1;

        Actions.OnResetValues?.Invoke();
        Actions.OnSetUiLocation(Page.LevelSelect);
        MenuVirtualCamera.OnResetCamera?.Invoke();
    }

    private void Intro()
    {
        Actions.OnSetUiLocation(Page.Intro);
    }

    private void Loading(bool isLoadingIntoDay)
    {
        loadingScreen.enabled = true;
        loadingText.text = "Loading...";

        if (isLoadingIntoDay)
        {
            Actions.OnActivateHowToPlay?.Invoke(true);
            fakeProgressSpeed = 0.2f;
        }
        else
        {
            fakeProgressSpeed = 0.6f;
        }
    }

    private void Gameplay()
    {
        MenuVirtualCamera.OnResetCamera?.Invoke();
        Actions.OnSetUiLocation(Page.Gameplay);
        Time.timeScale = 1;
    }

    private void EndOfDay()
    {
        Actions.OnSetUiLocation(Page.EndOfDay);
    }

    private void Pause()
    {
        Actions.OnSetUiLocation(Page.Pause);
        Time.timeScale = 0;
    }

    private void Settings()
    {
        settingsManager.OpenSettings();
    }

    #endregion

    // How to play
    public void ActivateHowToPlay(bool isLoading)
    {
        if (isLoading)
        {
            howToPlayPanelUI.SetActive(true);
            howToPlayBg.enabled = false;
            howToPlayBack.gameObject.SetActive(false);
        }
        else
        {
            howToPlayBg.enabled = true;
            howToPlayBack.gameObject.SetActive(true);
        }
    }

    private void DeactivateHowToPlay()
    {
        howToPlayPanelUI.SetActive(false);
    }
}

[Serializable]
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