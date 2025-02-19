using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState { MainMenu, Loading, Intro, LevelSelect, Gameplay, EndOfDay, Pause, Settings}
    public GameState gameState;
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
        if(SceneManager.GetActiveScene().name == "MainMenu")
            SetState(GameState.MainMenu);
        else
            SetState(GameState.Gameplay);
    }


    private void OnEnable()
    {
        Actions.OnForceStateChange += LoadState;
        InputManager.PauseAction += EscapeState;
    }

    private void OnDisable()
    {
        Actions.OnForceStateChange -= LoadState;
        InputManager.PauseAction -= EscapeState;
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
    private void EscapeState(InputAction.CallbackContext input)
    {
        if(input.performed && (gameState == GameState.Gameplay || gameState == GameState.Pause))
        {
            switch (gameState)
            {
                case GameState.Pause: SetState(GameState.Gameplay); break;
                case GameState.Gameplay: SetState(GameState.Pause); break;
            }
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}