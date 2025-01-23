using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState { MainMenu, Intro, LevelSelect, Gameplay, EndOfDay, Pause, Settings, GameOver }
    public GameState gameState;
    private GameState currentState;
    private GameState _previousState;
    private GameState _newState;


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
        SetState(GameState.Gameplay);
        //SetState(GameState.MainMenu);
        //currentState = gameState;
    }


    private void OnEnable()
    {
        Actions.OnPause += EscapeState;
        Actions.OnForceStateChange += LoadState;
    }

    private void OnDisable()
    {
        Actions.OnPause -= EscapeState;
        Actions.OnForceStateChange -= LoadState;
    }

    // This should be used for buttons
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


    private void SetState(GameState state)
    {
        if (state == GameState.Settings)
            _previousState = gameState;

        gameState = state;

        Actions.OnStateChange?.Invoke(gameState);
    }


    // this should be called when hitting the pause button. 
    private void EscapeState()
    {
        switch (gameState)
        {
            case GameState.Pause: SetState(GameState.Gameplay); break;
            case GameState.Gameplay: SetState(GameState.Pause); break;
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }

}