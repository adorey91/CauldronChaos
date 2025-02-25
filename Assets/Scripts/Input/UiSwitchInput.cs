using UnityEngine;
using UnityEngine.InputSystem;

public class UiSwitchInput : MonoBehaviour
{
    private void Awake()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDestroy()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void Start()
    {
        foreach(InputDevice input in InputSystem.devices)
        {
            Debug.Log(input);
        }
    }

    private void OnDeviceChange(InputDevice input, InputDeviceChange change)
    {
        switch(change)
        {
            case InputDeviceChange.Added:
                break;
            case InputDeviceChange.Disconnected:
                break;
        }
    }
}
