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
    [SerializeField] private Canvas gameOver;
    [SerializeField] private Canvas pause;

    [Header("Day Start Overlay")]
    [SerializeField] private GameObject dayStartPanel;
    [SerializeField] private TextMeshProUGUI dayStartText;
    [SerializeField] private int secondsToStart;
    private bool newDay;
    private int timer;


    // used for testing
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            newDay = true;
        }

        if (newDay)
        {
            dayStartPanel.SetActive(true);
            StartCoroutine(DayStartTimer());
        }
    }


    private void OnEnable()
    {
        Actions.OnStateChange += UpdateUIForGameState;
    }

    private void OnDisable()
    {
        Actions.OnStateChange -= UpdateUIForGameState;
    }

    private void UpdateUIForGameState(GameManager.GameState state)
    {
        switch (state)
        {
            case GameManager.GameState.MainMenu: MainMenu(); break;
            case GameManager.GameState.LevelSelect: LevelSelect(); break;
            case GameManager.GameState.Intro: Intro(); break;
            case GameManager.GameState.Gameplay: Gameplay(); break;
            case GameManager.GameState.EndOfDay: EndOfDay(); break;
            case GameManager.GameState.Pause: Pause(); break;
            case GameManager.GameState.Settings: Settings(); break;
            case GameManager.GameState.GameOver: GameOver(); break;
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
        gameOver.enabled = false;
        endOfDay.enabled = false;

        canvas.enabled = true;
    }


    // State UI Changes.
    #region State_UI_Changes
    private void MainMenu()
    {
        SetActiveUI(mainMenu);
        Actions.OnFirstSelect("Menu");
    }

    private void Intro()
    {
        SetActiveUI(intro);
        Actions.OnFirstSelect("Menu");
    }

    private void LevelSelect()
    {
        newDay = true;
        SetActiveUI(levelSelect);
        Actions.OnFirstSelect("Menu");
    }


    private void Gameplay()
    {
        SetActiveUI(gameplay);

        if (newDay)
        {
            dayStartPanel.SetActive(true);
            StartCoroutine(DayStartTimer());
        }


        Actions.OnFirstSelect("Menu");
        Time.timeScale = 1;
    }

    private void EndOfDay()
    {
        SetActiveUI(endOfDay);
        Actions.OnFirstSelect("Menu");
    }
    private void GameOver()
    {
        SetActiveUI(gameOver);
        Actions.OnFirstSelect("Menu");
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

    private IEnumerator DayStartTimer()
    {
        timer = secondsToStart;

        dayStartText.text = $"Day Starts in... {timer}";

        float normalizedTime = 0;
        while (normalizedTime <= secondsToStart)
        {

            dayStartText.text = $"Day Starts in... {normalizedTime}";
            normalizedTime += Time.deltaTime;
            yield return null;
        }

        newDay = false;
    }
}
