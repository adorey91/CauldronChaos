using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Start()
    {
        recipeBookUI.SetActive(false);
    }

    // I havent decided if this is a good way to deal with opening and closing logic.
    private void OnEnable()
    {
        Actions.ToggleRecipeBook += ToggleRecipeBook;
    }

    private void OnDisable()
    {
        Actions.ToggleRecipeBook -= ToggleRecipeBook;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && GameManager.instance.gameState == GameManager.GameState.Gameplay)
        {
            ToggleRecipeBook();
        }
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
    internal void MainMenu() => SetActiveUI(mainMenu);
    internal void Gameplay() => SetActiveUI(gameplay);
    internal void EndOfDay() => SetActiveUI(endOfDay);
    internal void GameOver() => SetActiveUI(gameOver);
    internal void Pause() => SetActiveUI(pause);
    internal void Settings()
    {
        SetActiveUI(settings);
        SetActiveSettings(settingsPanel);
    }
    #endregion

    // Settings UI Changes. Made public so they can be accessed in the inspector.
    #region Settings_UI
    public void OpenSettings() => SetActiveSettings(settingsPanel);
    public void OpenAudio() => SetActiveSettings(audioPanel);
    public void OpenVideo() => SetActiveSettings(videoPanel);
    public void OpenControls() => SetActiveSettings(controlsPanel);
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
}
