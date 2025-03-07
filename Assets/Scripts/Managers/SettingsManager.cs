using System.Collections.Generic;
using UnityEngine;
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
    private Dictionary<Button, (GameObject panel, Page page)> _menuMap;
    private int _currentMenuIndex;

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

    [Header("Settings Back Buttons")]
    [SerializeField] private Button settingsBack;

    [SerializeField] private Button debugBack;
    [SerializeField] private Button deleteYes;
    [SerializeField] private Button deleteNo;

    private Gamepad _gamepad;
    private bool _inSettings;

    private void Awake()
    {
        _menuMap = new Dictionary<Button, (GameObject, Page)>
        {
            { audioButton, (audioPanel, Page.Audio) },
            { videoButton, (videoPanel, Page.Video) },
            { controlsButton, (controlsPanel, Page.ControlsKeyboard) },
            { systemButton, (systemPanel, Page.System) },
            { deleteFileButton, (deleteFilePanel, Page.DeleteFile) },
            { debugButton, (debugPanel, Page.DebugInput) },
        };
    }

    private void Start()
    {
        _gamepad = Gamepad.current;

        foreach (var pair in _menuMap)
        {
            pair.Key.onClick.AddListener(() => OpenPanel(pair.Value.panel, pair.Value.page));
        }

        controlChangeButton.onClick.AddListener(ToggleControlScheme);
        settingsBack.onClick.AddListener(CloseSettings);
        debugBack.onClick.AddListener(() => OpenPanel(systemPanel, Page.System));
        deleteYes.onClick.AddListener(DeleteFile);
        deleteNo.onClick.AddListener(() => OpenPanel(systemPanel, Page.System));

        _currentMenuIndex = 0;
    }

    private void Update()
    {
        if (!_inSettings || _gamepad == null) return;

        if (_gamepad.rightShoulder.wasPressedThisFrame) CycleMenu(1);
        if (_gamepad.leftShoulder.wasPressedThisFrame) CycleMenu(-1);
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

    private void OpenPanel(GameObject panel,  Page page)
    {
        foreach (var p in _menuMap.Values) p.panel.SetActive(false);
        panel.SetActive(true);
        Actions.OnSetUiLocation(page);
    }

    private void CycleMenu(int direction)
    {
        var menuList = new List<(GameObject panel, Page page)>(_menuMap.Values);
        var buttonList = new List<Button>(_menuMap.Keys);

        do
        {
            _currentMenuIndex = (_currentMenuIndex + direction + menuList.Count) % menuList.Count;
        } 
        while (!ShouldShowMenu(_currentMenuIndex));

        OpenPanel(menuList[_currentMenuIndex].panel, menuList[_currentMenuIndex].page);
        buttonList[_currentMenuIndex].Select();
    }


    private bool ShouldShowMenu(int index)
    {
        var menuValues = new List<(GameObject panel, Page page)>(_menuMap.Values);
        return !(menuValues[index].panel == systemPanel && SceneManager.GetActiveScene().name != "MainMenu");
    }


    internal void OpenSettings()
    {
        systemButton.gameObject.SetActive(SceneManager.GetActiveScene().name == "MainMenu");
        OpenPanel(audioPanel, Page.Audio);
        _inSettings = true;
    }

    private void ToggleControlScheme()
    {
        var usingKeyboard = keyboardPanel.activeSelf;
        keyboardPanel.SetActive(!usingKeyboard);
        controllerPanel.SetActive(usingKeyboard);
    }
    private void DeleteFile()
    {
        Actions.OnDeleteSaveFile?.Invoke();
        OpenSettings();
    }

    private void CloseSettings()
    {
        Actions.OnStateChange("previousState");
        _inSettings = false;
    }
}