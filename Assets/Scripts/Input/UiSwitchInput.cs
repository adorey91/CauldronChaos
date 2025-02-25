using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UiSwitchInput : MonoBehaviour
{
    private UiObject.Page currentLocation;

    public static Action<InputAction.CallbackContext> MouseAction;
    public static Action<InputAction.CallbackContext> KeyboardAction;

    private PlayerInput playerControls;
    private InputAction MouseInputAction;
    private InputAction KeyboardInputAction;

    private bool isKeyboardControlling = false;
    private bool isMouseControlling = false;
    private bool isControllerControlling = false;

    private void Awake()
    {
        Actions.OnSetUiLocation += SetCurrentLocation;
    }

    private void OnDestroy()
    {
        Actions.OnSetUiLocation -= SetCurrentLocation;
    }

    private void Start()
    {
        playerControls = GetComponent<PlayerInput>();
        MouseInputAction = playerControls.actions.FindAction("MouseUsed");
        KeyboardInputAction = playerControls.actions.FindAction("KeyboardUsed");
    }

    

    private void SetCurrentLocation(UiObject.Page newLocation)
    {
        currentLocation = newLocation;
    }

    public void CheckForMouse(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            if (isMouseControlling) return;
            if (currentLocation == UiObject.Page.Gameplay) return;

            Debug.Log("Mouse is now navigating");

            isMouseControlling = true;
            isKeyboardControlling = false;

            Cursor.lockState = CursorLockMode.None;

            Actions.OnRemoveSelection?.Invoke();
        }
    }

    public void CheckForKeyboard(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            if (isKeyboardControlling) return;

            Cursor.lockState = CursorLockMode.Locked;

            Debug.Log("Keyboard is now navigating");
            isKeyboardControlling = true;
            isMouseControlling = false;

            Actions.OnFirstSelect?.Invoke();
        }
    }
}
