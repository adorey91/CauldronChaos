using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState { MainMenu, Gameplay, EndOfDay, Pause, Settings, GameOver }
    public GameState gameState;
    private GameState _previousState;
    private GameState _newState;

    [Header("Managers")]
    [SerializeField] private UiManager uiManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        SetState(GameState.MainMenu);
    }

    public void LoadState(string state)
    {
        if (state == "previousState")
            _newState = _previousState;
        else
        {
            if (Enum.TryParse(state, out GameState gamestate))
                _newState = gamestate;
            else
                Debug.LogError(state + " doesn't exist");
        }

        SetState(_newState);
    }

    // this should be called when hitting the pause button. 
    public void EscapeState()
    {
        switch (gameState)
        {
            case GameState.Pause: SetState(GameState.Gameplay); break;
            case GameState.Gameplay: SetState(GameState.Pause); break;
        }
    }


    private void SetState(GameState state)
    {
        if (state == GameState.Settings)
            _previousState = gameState;

        gameState = state;

        switch (gameState)
        {
            case GameState.MainMenu: MainMenu(); break;
            case GameState.Gameplay: Gameplay(); break;
            case GameState.EndOfDay: EndOfDay(); break;
            case GameState.Pause: Pause(); break;
            case GameState.Settings: Settings(); break;
            case GameState.GameOver: GameOver(); break;
        }
    }

    private void MainMenu()
    {
        Time.timeScale = 0;
        uiManager.MainMenu();
    }

    private void Gameplay()
    {
        Time.timeScale = 1;
        uiManager.Gameplay();
    }

    private void EndOfDay()
    {
        Time.timeScale = 0;
        uiManager.EndOfDay();
    }

    private void Pause()
    {
        Time.timeScale = 0;
        uiManager.Pause();
    }

    private void Settings()
    {
        uiManager.Settings();
    }

    private void GameOver()
    {
        Time.timeScale = 0;
        uiManager.GameOver();
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }

}
