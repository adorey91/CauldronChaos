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
    [SerializeField] private GameObject videoFirstSelect;
    [SerializeField] private GameObject controlsFirstSelect;
    [SerializeField] private GameObject endOfDayFirstSelect;
    [SerializeField] private GameObject introFirstSelect;
    [SerializeField] private GameObject deleteFileFirstSelect;

    [SerializeField] private EventSystem eventSystem;

    private GameObject _firstSelected;

    private void OnEnable()
    {
        Actions.OnFirstSelect += SetFirstSelect;
    }

    private void OnDisable()
    {
        Actions.OnFirstSelect -= SetFirstSelect;
    }


    internal bool IsControllerConnected()
    {
        return Gamepad.all.Count > 0;
    }

    public void SetControllerFirstSelect()
    {
        SetFirstSelect("DeleteFile");
    }

    private void SetFirstSelect(string menu)
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
            case "Video": _firstSelected = videoFirstSelect; break;
            case "Intro": _firstSelected = introFirstSelect; break;
            case "Controls": _firstSelected = controlsFirstSelect; break;
            case "EndOfDay": _firstSelected = endOfDayFirstSelect; break;
            case "DeleteFile": _firstSelected = deleteFileFirstSelect; break;
            default: Debug.LogWarning("First Select is not set for: " + menu); break;
        }

        eventSystem.SetSelectedGameObject(_firstSelected, new BaseEventData(eventSystem));
    }
}
