using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Settings Panels")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject videoPanel;
    [SerializeField] private GameObject controlsPanel;

    [Header("Settings Buttons")]
    [SerializeField] private Button[] settingsButton;
    [SerializeField] private Button audioButton;
    [SerializeField] private Button videoButton;
    [SerializeField] private Button controlsButton;

    [Header("Settings Back Buttons")]
    [SerializeField] private Button settingsBack;
    [SerializeField] private Button audioBack;
    [SerializeField] private Button videoBack;
    [SerializeField] private Button controlsBack;
    [SerializeField] private Button deleteYes;
    [SerializeField] private Button deleteNo;

    private void Start()
    {
        foreach (Button settings in settingsButton)
        {
            settings.onClick.AddListener(OpenSettings);
        }
        audioButton.onClick.AddListener(OpenAudio);
        videoButton.onClick.AddListener(OpenVideo);
        controlsButton.onClick.AddListener(OpenControls);

        settingsBack.onClick.AddListener(ReturnFromSettings);
        audioBack.onClick.AddListener(OpenSettings);
        videoBack.onClick.AddListener(OpenSettings);
        controlsBack.onClick.AddListener(OpenSettings);
        deleteYes.onClick.AddListener(OpenSettings);
        deleteNo.onClick.AddListener(OpenSettings);
    }


    private void ActivatePanel(GameObject _panel)
    {
        settingsPanel.SetActive(false);
        audioPanel.SetActive(false);
        videoPanel.SetActive(false);
        controlsPanel.SetActive(false);

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
        ActivatePanel(audioPanel);
        ControllerSelect.OnFirstSelect?.Invoke("Audio");
    }

    private void OpenVideo()
    {
        ActivatePanel(videoPanel);
        ControllerSelect.OnFirstSelect?.Invoke("Video");
    }
    private void OpenControls()
    {
        ActivatePanel(controlsPanel);
        ControllerSelect.OnFirstSelect?.Invoke("Controls");
    }

    private void ReturnFromSettings()
    {
        Actions.OnForceStateChange("previousState");
    }
}
