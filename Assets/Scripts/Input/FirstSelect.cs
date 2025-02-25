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
    [SerializeField] private GameObject debugToggleSelect;

    [SerializeField] private EventSystem eventSystem;

    private GameObject _firstSelected;

    #region OnEnable / OnDisable / OnDestroy Events
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

    private void OnDestroy()
    {
        Actions.OnFirstSelect -= SetFirstSelect;
        Actions.OnSelectRecipeButton -= SetRecipeBookButton;
    }
    #endregion

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

        _firstSelected = menu switch
        {
            "Menu" => menuFirstSelect,
            "Pause" => pauseFirstSelect,
            "LevelSelect" => levelSelectFirstSelect,
            "Settings" => settingsFirstSelect,
            "Audio" => audioFirstSelect,
            "Gameplay" => null,
            "Intro" => introFirstSelect,
            "Controls" => controlsFirstSelect,
            "EndOfDay" => endOfDayFirstSelect,
            "DeleteFile" => deleteFileFirstSelect,
            "HowToPlay" => howToPlayFirstSelect,
            "Debug" => debugFirstSelect,
            "DebugToggle" => debugToggleSelect,
            _ => null,
        };
        eventSystem.SetSelectedGameObject(_firstSelected, new BaseEventData(eventSystem));
    }
}
