using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Settings Panels")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject audioVideoPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject deleteFilePanel;
    [SerializeField] private GameObject debugPanel;

    [Header("Settings Buttons")]
    [SerializeField] private Button[] settingsButton;
    [SerializeField] private Button audioVideoButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button deleteFileButton;
    [SerializeField] private Button debugButton;

    [Header("Settings Back Buttons")]
    [SerializeField] private Button settingsBack;
    [SerializeField] private Button audioVideoBack;
    [SerializeField] private Button controlsBack;
    [SerializeField] private Button deleteYes;
    [SerializeField] private Button deleteNo;

    public static Action OpenSettingsAction;

    private void Start()
    {
        foreach (Button settings in settingsButton)
        {
            settings.onClick.AddListener(OpenSettings);
        }
        audioVideoButton.onClick.AddListener(OpenAudio);
        controlsButton.onClick.AddListener(OpenControls);
        deleteFileButton.onClick.AddListener(OpenDeleteFile);
        debugButton.onClick.AddListener(OpenDebug);

        settingsBack.onClick.AddListener(ReturnFromSettings);
        audioVideoBack.onClick.AddListener(OpenSettings);
        controlsBack.onClick.AddListener(OpenSettings);
        deleteYes.onClick.AddListener(DeleteFileYesButton);
        deleteNo.onClick.AddListener(OpenSettings);
    }

    private void OnEnable()
    {
        OpenSettingsAction += OpenSettings;
    }

    private void OnDisable()
    {
        OpenSettingsAction -= OpenSettings;
    }


    private void ActivatePanel(GameObject _panel)
    {
        settingsPanel.SetActive(false);
        audioVideoPanel.SetActive(false);
        controlsPanel.SetActive(false);
        deleteFilePanel.SetActive(false);

        _panel.SetActive(true);
    }

    // Settings UI Changes. Made public so they can be accessed in the inspector.
    internal void OpenSettings()
    {
        ActivatePanel(settingsPanel);
        ControllerSelect.OnFirstSelect?.Invoke("Settings");
    }

    private void OpenAudio()
    {
        ActivatePanel(audioVideoPanel);
        ControllerSelect.OnFirstSelect?.Invoke("Audio");
    }
  
    private void OpenControls()
    {
        ActivatePanel(controlsPanel);
        ControllerSelect.OnFirstSelect?.Invoke("Controls");
    }

    private void OpenDeleteFile()
    {
        ActivatePanel(deleteFilePanel);
        ControllerSelect.OnFirstSelect?.Invoke("DeleteFile");
    }

    private void OpenDebug()
    {
        ActivatePanel(debugPanel);
        //ControllerSelect.OnFirstSelect?.Invoke("Debug");
    }

    private void DeleteFileYesButton()
    {
        SaveManager.OnDeleteSaveFile?.Invoke();
        OpenSettings();
    }

    private void ReturnFromSettings()
    {
        Actions.OnForceStateChange("previousState");
    }
}
