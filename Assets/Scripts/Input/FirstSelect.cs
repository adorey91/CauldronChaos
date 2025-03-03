using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


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
    [SerializeField] private GameObject debugFirstSelect;
    [SerializeField] private GameObject debugToggleSelect;
    [SerializeField] private GameObject systemFirstSelect;
    #endregion

    [SerializeField] private EventSystem eventSystem;

    private GameObject firstSelected;
    private UiObject.Page currentLocation;

    private bool isKeyboardControlling = false;
    private bool isMouseControlling = false;
    private bool isControllerControlling = false;

    private bool isInRecipeBook = false;
    private GameObject recipeBookButton;

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

        isControllerControlling = Gamepad.current != null;
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

    }

    private void OnDisable()
    {
        Actions.OnSelectRecipeButton -= SetRecipeBookButton;
        Actions.OnSetUiLocation -= SetUiLocation;
    }

    private void OnDestroy()
    {
        Actions.OnSelectRecipeButton -= SetRecipeBookButton;
        Actions.OnSetUiLocation -= SetUiLocation;
        InputSystem.onDeviceChange -= OnDeviceChange;
    }
    #endregion


    #region First Selected
    // Sets the recipe book button for the controller
    private void SetRecipeBookButton(GameObject button)
    {
        isInRecipeBook = true;
        recipeBookButton = button;

        if (isKeyboardControlling || isControllerControlling)
        {
            eventSystem.SetSelectedGameObject(null);
            eventSystem.SetSelectedGameObject(button, new BaseEventData(eventSystem));
        }
    }

    private void SetInsideRecipeBook()
    {
        if (recipeBookButton == null) return;

        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(recipeBookButton, new BaseEventData(eventSystem));
    }

    // Sets the first selected button for the controller depending on the string
    public void SetFirstSelect()
    {
        OnRemoveSelection();
        if (isMouseControlling) return;
        if (currentLocation == UiObject.Page.Gameplay) return;

        switch (currentLocation)
        {
            case UiObject.Page.MainMenu: firstSelected = menuFirstSelect; break;
            case UiObject.Page.Intro: firstSelected = introFirstSelect; break;
            case UiObject.Page.LevelSelect: firstSelected = levelSelectFirstSelect; break;
            case UiObject.Page.Pause: firstSelected = pauseFirstSelect; break;
            case UiObject.Page.Settings: firstSelected = settingsFirstSelect; break;
            case UiObject.Page.EndOfDay: firstSelected = endOfDayFirstSelect; break;
            case UiObject.Page.Audio: firstSelected = audioFirstSelect; break;
            case UiObject.Page.ControlsKeyboard: firstSelected = controlsKeyFirstSelect; break;
            case UiObject.Page.ControlsGamepad: firstSelected = controlsGameFirstSelect; break;
            case UiObject.Page.DeleteFile: firstSelected = deleteFileFirstSelect; break;
            case UiObject.Page.HowToPlay: firstSelected = howToPlayFirstSelect; break;
            case UiObject.Page.DebugInput: firstSelected = debugFirstSelect; break;
            case UiObject.Page.DebugToggle: firstSelected = debugToggleSelect; break;
            case UiObject.Page.System: firstSelected = systemFirstSelect; break;
            case UiObject.Page.Video: firstSelected = videoFirstSelect; break;
            default:
                Debug.LogWarning("No matching case for currentLocation: " + currentLocation);
                firstSelected = null;
                break;
        }

        //Debug.Log($"First Selected: {firstSelected} for location {currentLocation}");

        if (firstSelected != null)
        {
            eventSystem.SetSelectedGameObject(firstSelected, new BaseEventData(eventSystem));
        }
        else
        {
            Debug.LogWarning("First Selected object is null!");
        }
    }


    private void OnRemoveSelection()
    {
        isInRecipeBook = false;
        eventSystem.SetSelectedGameObject(null);
    }
    #endregion

    private void SetUiLocation(UiObject.Page newLocation)
    {
        Debug.Log($"Switching UI location: {currentLocation} -> {newLocation}");
        currentLocation = newLocation;

        OnRemoveSelection();

        if (isKeyboardControlling || isControllerControlling)
            SetFirstSelect();
    }


    public void CheckForMouse()
    {
        if (Mouse.current == null) return;
        var mouse = Mouse.current;

        if (isMouseControlling) return;

        if (mouse?.delta.magnitude > 0.1f)
        {
            if (currentLocation == UiObject.Page.Gameplay) return;

            //Debug.Log("Mouse is now navigating");

            isMouseControlling = true;
            isKeyboardControlling = false;
            isControllerControlling = false;

            Cursor.lockState = CursorLockMode.None;

            OnRemoveSelection();
        }
    }

    public void CheckForKeyboard()
    {
        if (Keyboard.current == null) return;
        var keyboard = Keyboard.current;

        if (isKeyboardControlling) return;

        if (keyboard?.anyKey.wasPressedThisFrame == true)
        {

            Cursor.lockState = CursorLockMode.Locked;

            //Debug.Log("Keyboard is now navigating");
            isKeyboardControlling = true;
            isMouseControlling = false;
            isControllerControlling = false;

            if (isInRecipeBook)
            {
                SetInsideRecipeBook();
                return;
            }
            else
            {
                SetFirstSelect();
            }
        }
    }

    public void CheckForController()
    {
        if (Gamepad.current == null) return;
        var gamepad = Gamepad.current;

        if (isControllerControlling) return;

        if (gamepad.buttonSouth.wasPressedThisFrame ||
           gamepad.buttonNorth.wasPressedThisFrame ||
           gamepad.buttonEast.wasPressedThisFrame ||
           gamepad.buttonWest.wasPressedThisFrame ||
           gamepad.leftStick.ReadValue().magnitude > 0.1f ||
           gamepad.rightStick.ReadValue().magnitude > 0.1f ||
           gamepad.dpad.ReadValue().magnitude > 0.1f)
        {
            Cursor.lockState = CursorLockMode.Locked;

            //Debug.Log("Controller is now navigating");
            isControllerControlling = true;
            isMouseControlling = false;
            isKeyboardControlling = false;
            SetFirstSelect();
        }

    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is Gamepad)
        {
            switch (change)
            {
                case InputDeviceChange.Added:
                    isControllerControlling = true;
                    isMouseControlling = false;
                    isKeyboardControlling = false;
                    SetFirstSelect();
                    break;
                case InputDeviceChange.Removed:
                    isControllerControlling = false;
                    OnRemoveSelection();
                    break;
            }
        }
    }
}
