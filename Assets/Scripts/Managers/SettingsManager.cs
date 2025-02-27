using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Settings Panels")]
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject videoPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject systemPanel;
    [SerializeField] private GameObject deleteFilePanel;
    [SerializeField] private GameObject debugPanel;
    private GameObject[] menuPanels;
    private int currentMenuIndex = 0;

    [Header("Controls")]
    [SerializeField] private GameObject controllerPanel;
    [SerializeField] private GameObject keyboardPanel;
    [SerializeField] private Button controlChangeButton;

    [Header("Settings Buttons")]
    [SerializeField] private Button[] settingsButton;
    [SerializeField] private Button audioButton;
    [SerializeField] private Button videoButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button systemButton;
    [SerializeField] private Button deleteFileButton;
    [SerializeField] private Button debugButton;
    private Button[] menuButtons;

    [Header("Settings Back Buttons")]
    [SerializeField] private Button settingsBack;
    [SerializeField] private Button debugBack;
    [SerializeField] private Button deleteYes;
    [SerializeField] private Button deleteNo;

    private bool inSettings = false;

    private void Start()
    {
        menuPanels = new GameObject[] { audioPanel, videoPanel, controlsPanel, systemPanel};
        menuButtons = new Button[] { audioButton, videoButton, controlsButton, systemButton };


        foreach (Button settings in settingsButton)
        {
            settings.onClick.AddListener(OpenSettings);
        }
        audioButton.onClick.AddListener(OpenAudio);
        videoButton.onClick.AddListener(OpenVideo);
        controlsButton.onClick.AddListener(OpenControls);
        deleteFileButton.onClick.AddListener(OpenDeleteFile);
        debugButton.onClick.AddListener(OpenDebug);
        systemButton.onClick.AddListener(OpenSystem);

        controlChangeButton.onClick.AddListener(ChangeControls);

        settingsBack.onClick.AddListener(ReturnFromSettings);
        debugBack.onClick.AddListener(OpenSystem);
        deleteYes.onClick.AddListener(DeleteFileYesButton);
        deleteNo.onClick.AddListener(OpenSystem);
    }

    private void Update()
    {
        if(!inSettings) return;
        if (Gamepad.current.rightShoulder.wasPressedThisFrame)
        {
            CycleMenu(1);
        }
        if (Gamepad.current.leftShoulder.wasPressedThisFrame)
        {
            CycleMenu(-1);
        }
    }

    #region OnEnable / OnDisable / OnDestroy Events
    private void OnEnable()
    {
        Actions.OnOpenSettingsAction += OpenSettings;
    }

    private void OnDisable()
    {
        Actions.OnOpenSettingsAction -= OpenSettings;
    }

    private void OnDestroy()
    {
        Actions.OnOpenSettingsAction -= OpenSettings;
    }
    #endregion

    private void ActivatePanel(GameObject _panel)
    {
        audioPanel.SetActive(false);
        videoPanel.SetActive(false);
        controlsPanel.SetActive(false);
        deleteFilePanel.SetActive(false);
        systemPanel.SetActive(false);
        debugPanel.SetActive(false);
        videoPanel.SetActive(false);

        _panel.SetActive(true);
    }

    private void CycleMenu(int direction)
    {
        int previousIndex = currentMenuIndex;

        do
        {
            currentMenuIndex += direction;

            if(currentMenuIndex >= menuPanels.Length) currentMenuIndex = 0;
            if(currentMenuIndex < 0) currentMenuIndex = menuPanels.Length - 1;
        } while (!ShouldShowMenu(currentMenuIndex));

        // activate the new panel
        ActivatePanel(menuPanels[currentMenuIndex]);
        menuButtons[currentMenuIndex].Select();
    }

    private bool ShouldShowMenu(int index)
    {
        // if system panel is selected but the scene isnt the main menu, dont show it
        return !(menuPanels[index] == systemPanel && SceneManager.GetActiveScene().name != "MainMenu");
    }

    // Settings UI Changes. Made public so they can be accessed in the inspector.
    internal void OpenSettings()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
            systemButton.gameObject.SetActive(false);
        else
            systemButton.gameObject.SetActive(true);

        Actions.OnSetUiLocation(UiObject.Page.Settings);
        ActivatePanel(audioPanel);
        currentMenuIndex = 0;
        inSettings = true;
    }

    private void OpenAudio()
    {
        ActivatePanel(audioPanel);
        Actions.OnSetUiLocation(UiObject.Page.Audio);
    }
    private void OpenVideo()
    {
        ActivatePanel(videoPanel);
        Actions.OnSetUiLocation(UiObject.Page.Video);
    }

    private void OpenControls()
    {
        ActivatePanel(controlsPanel);
        OpenKeyboard();
    }

    private void OpenKeyboard()
    {
        Actions.OnSetUiLocation(UiObject.Page.ControlsKeyboard);
        controllerPanel.SetActive(false);
        keyboardPanel.SetActive(true);
        //controlsTitle.text = "Keyboard Controls";
    }

    private void OpenController()
    {
        Actions.OnSetUiLocation(UiObject.Page.ControlsGamepad);
        keyboardPanel.SetActive(false);
        controllerPanel.SetActive(true);
        //controlsTitle.text = "Controller Controls";
    }

    private void ChangeControls()
    {
        if (keyboardPanel.activeSelf)
            OpenController();
        else
            OpenKeyboard();
    }

    private void OpenSystem()
    {
        ActivatePanel(systemPanel);
        Actions.OnSetUiLocation(UiObject.Page.System);
    }

    private void OpenDeleteFile()
    {
        ActivatePanel(deleteFilePanel);
        Actions.OnSetUiLocation(UiObject.Page.DeleteFile);
    }

    private void OpenDebug()
    {
        ActivatePanel(debugPanel);
        Actions.OnSetUiLocation(UiObject.Page.DebugInput);
    }

    private void DeleteFileYesButton()
    {
        Actions.OnDeleteSaveFile?.Invoke();
        OpenSettings();
    }

    private void ReturnFromSettings()
    {
        Actions.OnForceStateChange("previousState");
        inSettings = false;
    }
}
