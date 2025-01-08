using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState { MainMenu, Gameplay, Pause, Settings, GameOver }
    public GameState gameState;
    private GameState previousState;

    [Header("Managers")]
    [SerializeField] private UiManager uiManager;

    private void Start()
    {
        SetState(GameState.MainMenu);
    }

    public void LoadState(string state)
    {
        GameState newState;
        if (state == "previousState")
            newState = previousState;
        else
            newState = (GameState)System.Enum.Parse(typeof(GameState), state);

        SetState(newState);
    }

    private void SetState(GameState state)
    {
        if (state == GameState.Settings)
            previousState = gameState;

        gameState = state;

        switch (gameState)
        {
            case GameState.MainMenu: MainMenu(); break;
            case GameState.Gameplay: Gameplay(); break;
            case GameState.Pause: Pause(); break;
            case GameState.Settings: Settings(); break;
            case GameState.GameOver: GameOver(); break;
        }
    }

    private void MainMenu()
    {
        uiManager.MainMenu();
    }

    private void Gameplay()
    {
        uiManager.Gameplay();
    }

    private void Pause()
    {
        uiManager.Pause();
    }

    private void Settings()
    {
        uiManager.Settings();
    }

    private void GameOver()
    {
        uiManager.GameOver();
    }
}
