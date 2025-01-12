using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UiManager : MonoBehaviour
{
    [Header("Ui Canvas")]
    [SerializeField] private Canvas mainMenu;
    [SerializeField] private Canvas gameplay;
    [SerializeField] private Canvas endOfDay;
    [SerializeField] private Canvas settings;
    [SerializeField] private Canvas gameOver;
    [SerializeField] private Canvas pause;

    [Header("Settings")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject videoPanel;
    [SerializeField] private GameObject controlsPanel;

    [Header("Recipe UI")]
    [SerializeField] private GameObject recipeBookUI;

    [Header("Event System")]
    [SerializeField] private EventSystem eventSystem;

    [Header("Ui First Selected")]
    [SerializeField] private GameObject menuFirstSelect;
    [SerializeField] private GameObject pauseFirstSelect;
    [SerializeField] private GameObject settingsFirstSelect;
    [SerializeField] private GameObject audioFirstSelect;
    [SerializeField] private GameObject videoFirstSelect;
    [SerializeField] private GameObject controlsFirstSelect;
    [SerializeField] private GameObject endOfDayFirstSelect;


    private void Start()
    {
        recipeBookUI.SetActive(false);
    }

    // I havent decided if this is a good way to deal with opening and closing logic.
    private void OnEnable()
    {
        Actions.OnToggleRecipeBook += ToggleRecipeBook;
    }

    private void OnDisable()
    {
        Actions.OnToggleRecipeBook -= ToggleRecipeBook;
    }

    // Toggles the current active canvas
    private void SetActiveUI(Canvas canvas)
    {
        mainMenu.enabled = false;
        gameplay.enabled = false;
        pause.enabled = false;
        settings.enabled = false;
        gameOver.enabled = false;
        endOfDay.enabled = false;

        // Clears selected button
        eventSystem.SetSelectedGameObject(null);
        // Makes sure that any settings panels open are closed
        SetActiveSettings(null);

        canvas.enabled = true;
    }

    // Toggles current active settings
    private void SetActiveSettings(GameObject panel)
    {
        settingsPanel.SetActive(false);
        audioPanel.SetActive(false);
        videoPanel.SetActive(false);
        controlsPanel.SetActive(false);

        if (panel != null)
            panel.SetActive(true);
    }

    // State UI Changes. Made internal as they don't need to be accessed in the inspector
    #region State_UI_Changes
    internal void MainMenu()
    {
        SetActiveUI(mainMenu);
        SetFirstSelect(menuFirstSelect);
    }
    internal void Gameplay() => SetActiveUI(gameplay);
    internal void EndOfDay()
    {
        SetActiveUI(endOfDay);
        SetFirstSelect(endOfDayFirstSelect);
    }
    internal void GameOver() => SetActiveUI(gameOver);
    internal void Pause()
    {
        SetActiveUI(pause);
        SetFirstSelect(pauseFirstSelect);
    }

    internal void Settings()
    {
        SetActiveUI(settings);
        SetActiveSettings(settingsPanel);
    }
    #endregion

    // Settings UI Changes. Made public so they can be accessed in the inspector.
    #region Settings_UI
    public void OpenSettings()
    {
        SetActiveSettings(settingsPanel);
        SetFirstSelect(settingsFirstSelect);

    }

    public void OpenAudio()
    {
        SetActiveSettings(audioPanel);
        SetFirstSelect(audioFirstSelect);
    }

    public void OpenVideo()
    {
        SetActiveSettings(videoPanel);
        SetFirstSelect(videoFirstSelect);
    }
    public void OpenControls()
    {
        SetActiveSettings(controlsPanel);
        SetFirstSelect(controlsFirstSelect);
    }
    #endregion

    public void ToggleRecipeBook()
    {
        if (recipeBookUI.activeSelf)
        {
            Time.timeScale = 1;
            recipeBookUI.SetActive(false);
        }
        else
        {
            Time.timeScale = 0;
            recipeBookUI.SetActive(true);
        }
    }

    private IEnumerator DelayedSetFirstSelect(GameObject firstSelect)
    {
        // Ensure the canvas and hierarchy are updated before selection
        yield return new WaitForEndOfFrame();

        eventSystem.SetSelectedGameObject(firstSelect); // Set the new selection
    }

    private void SetFirstSelect(GameObject firstSelect)
    {
        eventSystem.SetSelectedGameObject(null); // Clear current selection
        Canvas.ForceUpdateCanvases();
        if (Gamepad.all.Count == 0) return;

        StartCoroutine(DelayedSetFirstSelect(firstSelect));
    }
}
