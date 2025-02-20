using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ControllerSelect : MonoBehaviour
{
    [Header("Ui First Selected")]
    [SerializeField] private GameObject menuFirstSelect;
    [SerializeField] private GameObject pauseFirstSelect;
    [SerializeField] private GameObject settingsFirstSelect;
    [SerializeField] private GameObject levelSelectFirstSelect;
    [SerializeField] private GameObject audioFirstSelect;
    [SerializeField] private GameObject controlsFirstSelect;
    [SerializeField] private GameObject endOfDayFirstSelect;
    [SerializeField] private GameObject introFirstSelect;
    [SerializeField] private GameObject deleteFileFirstSelect;
    [SerializeField] private GameObject howToPlayFirstSelect;
    [SerializeField] private GameObject debugFirstSelect;

    [SerializeField] private EventSystem eventSystem;

    private GameObject _firstSelected;

    // Actions for controller select
    public static Action<GameObject> SelectRecipeButton;
    public static Action<string> OnFirstSelect;

    private void OnEnable()
    {
        OnFirstSelect += SetFirstSelect;
        SelectRecipeButton += SetRecipeBookButton;
    }

    private void OnDisable()
    {
        OnFirstSelect -= SetFirstSelect;
        SelectRecipeButton -= SetRecipeBookButton;
    }

    // Sets the recipe book button for the controller
    private void SetRecipeBookButton(GameObject button)
    {
        eventSystem.SetSelectedGameObject(null);

        if (!IsControllerConnected())
            return;

        eventSystem.SetSelectedGameObject(button, new BaseEventData(eventSystem));
    }

    // Checks if controller is connected
    internal bool IsControllerConnected()
    {
        return Gamepad.all.Count > 0;
    }

    // Sets the first selected button for the controller depending on the string
    public void SetFirstSelect(string menu)
    {
        eventSystem.SetSelectedGameObject(null);

        if (!IsControllerConnected())
            return;

        switch (menu)
        {
            case "Menu": _firstSelected = menuFirstSelect; break;
            case "Pause": _firstSelected = pauseFirstSelect; break;
            case "LevelSelect": _firstSelected = levelSelectFirstSelect; break;
            case "Settings": _firstSelected = settingsFirstSelect; break;
            case "Audio": _firstSelected = audioFirstSelect; break;
            case "Gameplay": _firstSelected = null; break;
            case "Intro": _firstSelected = introFirstSelect; break;
            case "Controls": _firstSelected = controlsFirstSelect; break;
            case "EndOfDay": _firstSelected = endOfDayFirstSelect; break;
            case "DeleteFile": _firstSelected = deleteFileFirstSelect; break;
            case "HowToPlay": _firstSelected = howToPlayFirstSelect; break;
            case "Debug": _firstSelected = debugFirstSelect; break;
            default:
                _firstSelected = null;
                break;
        }

        eventSystem.SetSelectedGameObject(_firstSelected, new BaseEventData(eventSystem));
    }
}
