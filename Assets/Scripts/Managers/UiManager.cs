using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [Header("Ui Canvas")]
    [SerializeField] private Canvas mainMenu;
    [SerializeField] private Canvas gameplay;
    [SerializeField] private Canvas settings;
    [SerializeField] private Canvas gameOver;
    [SerializeField] private Canvas pause;

    [Header("Settings")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject videoPanel;
    [SerializeField] private GameObject controlsPanel;

    private void SetActiveUI(Canvas canvas)
    {
        mainMenu.enabled = false;
        gameplay.enabled = false;
        pause.enabled = false;
        settings.enabled = false;
        gameOver.enabled = false;

        SetActiveSettings(null);

        canvas.enabled = true;
    }

    private void SetActiveSettings(GameObject panel)
    {
        settingsPanel.SetActive(false);
        audioPanel.SetActive(false);
        videoPanel.SetActive(false);
        controlsPanel.SetActive(false);

        if (panel != null)
            panel.SetActive(true);
    }

    internal void MainMenu() => SetActiveUI(mainMenu);
    internal void Gameplay() => SetActiveUI(gameplay);
    internal void GameOver() => SetActiveUI(gameOver);
    internal void Pause() => SetActiveUI(pause);
    internal void Settings()
    {
        SetActiveUI(settings);
        SetActiveSettings(settingsPanel);
    }
    public void OpenSettings() => SetActiveSettings(settingsPanel);
    public void OpenAudio() => SetActiveSettings(audioPanel);
    public void OpenVideo() => SetActiveSettings(videoPanel);
    public void OpenControls() => SetActiveSettings(controlsPanel);
}
