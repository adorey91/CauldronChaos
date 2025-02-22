using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class FirstSelect : MonoBehaviour
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
    
    private void OnEnable()
    {
        Actions.OnFirstSelect += SetFirstSelect;
        Actions.OnSelectRecipeButton += SetRecipeBookButton;
    }

    private void OnDisable()
    {
        Actions.OnFirstSelect -= SetFirstSelect;
        Actions.OnSelectRecipeButton -= SetRecipeBookButton;
    }

    // Sets the recipe book button for the controller
    private void SetRecipeBookButton(GameObject button)
    {
        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(button, new BaseEventData(eventSystem));
    }

  
    // Sets the first selected button for the controller depending on the string
    public void SetFirstSelect(string menu)
    {
        eventSystem.SetSelectedGameObject(null);

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
