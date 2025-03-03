using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum GameState {MainMenu, Loading, Intro, LevelSelect, Gameplay, EndOfDay, Pause, Settings}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameState gameState;
    internal GameState previousState;
    private GameState newState;

    private bool isInDebugMode = false;

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
        if (SceneManager.GetActiveScene().name == "MainMenu")
            SetState(GameState.MainMenu);
        else
            SetState(GameState.Gameplay);
    }

    #region OnEnable / OnDisable / OnDestroy Events
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

    private void OnDestroy()
    {
        Actions.OnForceStateChange -= LoadState;
        InputManager.PauseAction -= EscapeState;
    }
    #endregion

    // This should be used for buttons
    public void LoadState(string state)
    {
        if (state == "previousState")
            newState = previousState;
        else
        {
            if (Enum.TryParse(state, out GameState gamestate))
                newState = gamestate;
            else
                Debug.LogError(state + " doesn't exist");
        }

        SetState(newState);
    }


    private void SetState(GameState state)
    {
        if (state == GameState.Settings)
            previousState = gameState;

        //resetting music pitch after pause
        if (previousState == GameState.Pause && state != GameState.Settings)
        {
            AudioManager.instance.musicManager.musicSource.pitch = 1;
        }

        gameState = state;

        Song.SongType curSongType = AudioManager.instance.musicManager.GetCurrentMusic();
        
        switch (gameState)
        {
            case GameState.MainMenu:
                if (!(curSongType == Song.SongType.MainMenuMusic))
                {
                    AudioManager.instance.musicManager.PlayMusic(Song.SongType.MainMenuMusic);
                }
                break;

            case GameState.Gameplay:
                if (!(curSongType == Song.SongType.GameplayMusic))
                {
                    AudioManager.instance.musicManager.PlayMusic(Song.SongType.GameplayMusic);
                }
                break;

            case GameState.Pause:
                AudioManager.instance.musicManager.musicSource.pitch = 0.5f;
                break;

            case GameState.EndOfDay:
                if (!(curSongType == Song.SongType.MainMenuMusic))
                {
                    AudioManager.instance.musicManager.PlayMusic(Song.SongType.MainMenuMusic);
                }
                break;

            case GameState.LevelSelect:
                if (!(curSongType == Song.SongType.MainMenuMusic))
                {
                    AudioManager.instance.musicManager.PlayMusic(Song.SongType.MainMenuMusic);
                }
                break;
        }
        

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

    internal bool IsDebugging()
    {
        return isInDebugMode;
    }

    internal void SetDebugMode(bool debug)
    {
        isInDebugMode = debug;
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}