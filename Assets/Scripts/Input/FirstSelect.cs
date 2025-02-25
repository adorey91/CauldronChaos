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
    [SerializeField] private GameObject controlsKeyFirstSelect;
    [SerializeField] private GameObject controlsGameFirstSelect;
    [SerializeField] private GameObject endOfDayFirstSelect;
    [SerializeField] private GameObject introFirstSelect;
    [SerializeField] private GameObject deleteFileFirstSelect;
    [SerializeField] private GameObject howToPlayFirstSelect;
    [SerializeField] private GameObject debugFirstSelect;
    [SerializeField] private GameObject debugToggleSelect;

    [SerializeField] private EventSystem eventSystem;

    private GameObject firstSelected;
    private UiObject.Page currentLocation;

    #region OnEnable / OnDisable / OnDestroy Events
    private void OnEnable()
    {
        Actions.OnSelectRecipeButton += SetRecipeBookButton;
        Actions.OnSetUiLocation += SetUiLocation;
        Actions.OnFirstSelect += SetFirstSelect;
        Actions.OnRemoveSelection += OnRemoveSelection;
    }

    private void OnDisable()
    {
        Actions.OnSelectRecipeButton -= SetRecipeBookButton;
        Actions.OnSetUiLocation -= SetUiLocation;
        Actions.OnFirstSelect -= SetFirstSelect;
        Actions.OnRemoveSelection -= OnRemoveSelection;
    }

    private void OnDestroy()
    {
        Actions.OnSelectRecipeButton -= SetRecipeBookButton;
        Actions.OnSetUiLocation -= SetUiLocation;
        Actions.OnFirstSelect -= SetFirstSelect;
        Actions.OnRemoveSelection -= OnRemoveSelection;
    }
    #endregion

    // Sets the recipe book button for the controller
    private void SetRecipeBookButton(GameObject button)
    {
        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(button, new BaseEventData(eventSystem));
    }


    // Sets the first selected button for the controller depending on the string
    public void SetFirstSelect()
    {
        eventSystem.SetSelectedGameObject(null);

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
        }


        eventSystem.SetSelectedGameObject(firstSelected, new BaseEventData(eventSystem));
    }

    private void OnRemoveSelection()
    {
        eventSystem.SetSelectedGameObject(null);
    }

    private void SetUiLocation(UiObject.Page newLocation)
    {
        currentLocation = newLocation;
    }
}
