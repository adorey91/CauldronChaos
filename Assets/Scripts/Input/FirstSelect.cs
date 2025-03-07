using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System;


public class FirstSelect : MonoBehaviour
{
    #region Ui First Selected
    [Header("Ui First Selected")]
    [SerializeField] private GameObject menuFirstSelect;
    [SerializeField] private GameObject pauseFirstSelect;
    [SerializeField] private GameObject settingsFirstSelect;
    [SerializeField] private GameObject levelSelectFirstSelect;
    [SerializeField] private GameObject audioFirstSelect;
    [SerializeField] private GameObject videoFirstSelect;
    [SerializeField] private GameObject controlsKeyFirstSelect;
    [SerializeField] private GameObject controlsGameFirstSelect;
    [SerializeField] private GameObject endOfDayFirstSelect;
    [SerializeField] private GameObject introFirstSelect;
    [SerializeField] private GameObject deleteFileFirstSelect;
    [SerializeField] private GameObject howToPlayFirstSelect;
    [SerializeField] private GameObject debugInputFirstSelect;
    [SerializeField] private GameObject debugToggleSelect;
    [SerializeField] private GameObject systemFirstSelect;
    #endregion

    [SerializeField] private EventSystem eventSystem;

    private Page _currentLocation;
    private Dictionary<Page, Action> _selectActions;

    private bool _isKeyboardControlling;
    private bool _isMouseControlling;
    private bool _isControllerControlling;

    private bool _isInRecipeBook;
    private GameObject _recipeBookButton;

    private void Awake()
    {
        // Set dictionary to select the action based on the current page location
        _selectActions = new Dictionary<Page, Action>
        {
            { Page.MainMenu, () => SetFirstSelected(menuFirstSelect) },
            { Page.Intro, () => SetFirstSelected(introFirstSelect) },
            { Page.Settings, () => SetFirstSelected(settingsFirstSelect) },
            { Page.LevelSelect, () => SetFirstSelected(levelSelectFirstSelect) },
            { Page.Audio, () => SetFirstSelected(audioFirstSelect) },
            { Page.Video, () => SetFirstSelected(videoFirstSelect) },
            { Page.System, () => SetFirstSelected(systemFirstSelect) },
            { Page.ControlsGamepad, () => SetFirstSelected((controlsGameFirstSelect)) },
            { Page.EndOfDay, () => SetFirstSelected(endOfDayFirstSelect) },
            { Page.ControlsKeyboard, () => SetFirstSelected(controlsKeyFirstSelect) },
            { Page.DeleteFile, () => SetFirstSelected(deleteFileFirstSelect) },
            { Page.HowToPlay, () => SetFirstSelected(howToPlayFirstSelect) },
            { Page.DebugInput, () => SetFirstSelected(debugInputFirstSelect) },
            { Page.DebugToggle, () => SetFirstSelected(debugToggleSelect) },
        };
    }

    private void Start()
    {
        if (eventSystem == null)
        {
            eventSystem = FindObjectOfType<EventSystem>();
            if (eventSystem == null)
            {
                Debug.LogError("No EventSystem found in the scene!");
            }
        }

        _isControllerControlling = Gamepad.current != null;
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void Update()
    {
        CheckForController();
        CheckForKeyboard();
        CheckForMouse();
    }

    #region OnEnable / OnDisable / OnDestroy Events

    private void OnEnable()
    {
        Actions.OnSelectRecipeButton += SetRecipeBookButton;
        Actions.OnSetUiLocation += SetUiLocation;
        Actions.OnResetValues += ResetRecipeButton;
    }

    private void OnDisable()
    {
        Actions.OnSelectRecipeButton -= SetRecipeBookButton;
        Actions.OnSetUiLocation -= SetUiLocation;
        Actions.OnResetValues -= ResetRecipeButton;
    }

    // Is only being used as a backup if on disable doesn't get called or work.
    private void OnDestroy()
    {
        Actions.OnSelectRecipeButton -= SetRecipeBookButton;
        Actions.OnSetUiLocation -= SetUiLocation;
        InputSystem.onDeviceChange -= OnDeviceChange;
        Actions.OnResetValues -= ResetRecipeButton;
    }
    #endregion


    #region First Selected
    // Sets the recipe book button for the controller
    private void SetRecipeBookButton(GameObject button)
    {
        _isInRecipeBook = true;
        _recipeBookButton = button;

        if (_isKeyboardControlling || _isControllerControlling)
        {
            eventSystem.SetSelectedGameObject(null);
            eventSystem.SetSelectedGameObject(button, new BaseEventData(eventSystem));
        }
    }

    private void SetInsideRecipeBook()
    {
        if (_recipeBookButton == null) return;

        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(_recipeBookButton, new BaseEventData(eventSystem));
    }

    private void SetFirstSelected(GameObject button)
    {
        OnRemoveSelection();
        if (_isMouseControlling) return;

        eventSystem.SetSelectedGameObject(button, new BaseEventData(eventSystem));
    }

    private void OnRemoveSelection()
    {
        _isInRecipeBook = false;
        eventSystem.SetSelectedGameObject(null);
    }

    #endregion

    private void SetUiLocation(Page newLocation)
    {
        Debug.Log($"Switching UI location: {_currentLocation} -> {newLocation}");
        _currentLocation = newLocation;

        OnRemoveSelection();

        if (_isKeyboardControlling || _isControllerControlling)
            if (_selectActions.ContainsKey(_currentLocation))
                _selectActions[_currentLocation]();
    }

    private void ResetRecipeButton()
    {
        _recipeBookButton = null;
    }
    

    public void CheckForMouse()
    {
        if (Mouse.current == null) return;
        var mouse = Mouse.current;

        if (_isMouseControlling) return;

        if (mouse?.delta.magnitude > 0.1f)
        {
            if (_currentLocation == Page.Gameplay) return;

            //Debug.Log("Mouse is now navigating");

            _isMouseControlling = true;
            _isKeyboardControlling = false;
            _isControllerControlling = false;

            Cursor.lockState = CursorLockMode.None;

            OnRemoveSelection();
        }
    }

    public void CheckForKeyboard()
    {
        if (Keyboard.current == null) return;
        var keyboard = Keyboard.current;

        if (_isKeyboardControlling) return;

        if (keyboard?.anyKey.wasPressedThisFrame != true) return;

        Cursor.lockState = CursorLockMode.Locked;

        //Debug.Log("Keyboard is now navigating");
        _isKeyboardControlling = true;
        _isMouseControlling = false;
        _isControllerControlling = false;

        if (_isInRecipeBook)
        {
            SetInsideRecipeBook();
            return;
        }

        if (_selectActions.ContainsKey(_currentLocation))
            _selectActions[_currentLocation]();
    }

    private void CheckForController()
    {
        if (Gamepad.current == null) return;
        var gamepad = Gamepad.current;

        if (_isControllerControlling) return;

        if (!gamepad.buttonSouth.wasPressedThisFrame &&
            !gamepad.buttonNorth.wasPressedThisFrame &&
            !gamepad.buttonEast.wasPressedThisFrame &&
            !gamepad.buttonWest.wasPressedThisFrame &&
            !(gamepad.leftStick.ReadValue().magnitude > 0.1f) &&
            !(gamepad.rightStick.ReadValue().magnitude > 0.1f) &&
            !(gamepad.dpad.ReadValue().magnitude > 0.1f)) return;
        
        Cursor.lockState = CursorLockMode.Locked;

        //Debug.Log("Controller is now navigating");
        _isControllerControlling = true;
        _isMouseControlling = false;
        _isKeyboardControlling = false;
        if (_selectActions.ContainsKey(_currentLocation))
            _selectActions[_currentLocation]();
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is Gamepad)
        {
            switch (change)
            {
                case InputDeviceChange.Added:
                    _isControllerControlling = true;
                    _isMouseControlling = false;
                    _isKeyboardControlling = false;
                    if (_selectActions.ContainsKey(_currentLocation))
                        _selectActions[_currentLocation]();
                    break;
                case InputDeviceChange.Removed:
                    _isControllerControlling = false;
                    OnRemoveSelection();
                    break;
            }
        }
    }
}