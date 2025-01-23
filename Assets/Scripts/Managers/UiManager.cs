using UnityEngine;

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

    
    // I havent decided if this is a good way to deal with opening and closing logic.
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
        SetActiveUI(levelSelect);
        Actions.OnFirstSelect("Menu");
    }


    private void Gameplay()
    {
        SetActiveUI(gameplay);
        Actions.OnFirstSelect("Menu");
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
    }

    private void Settings()
    {
        SetActiveUI(settings);
        settingsManager.OpenSettings();
    }
    #endregion
}
