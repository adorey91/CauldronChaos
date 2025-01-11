using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GraphicsManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    private Resolution[] _resolutions;
    private List<Resolution> _filteredResolutions = new();

    private float _currentRefreshRate;
    private int _currentResolutionIndex;
    private bool _isFullScreen = true;

    // Start is called before the first frame update
    private void Start()
    {
        _resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        _currentRefreshRate = (float)Screen.currentResolution.refreshRateRatio.value;

        for (int i = 0; i < _resolutions.Length; i++)
        {
            if (_resolutions[i].refreshRateRatio.value == _currentRefreshRate)
                _filteredResolutions.Add(_resolutions[i]);
        }

        if (_filteredResolutions.Count == 0)
        {
            Debug.LogError("No resolutions found matching the current refresh rate.");
            return;
        }

        _filteredResolutions.Sort((a, b) =>
        {
            if (a.width != b.width)
                return b.width.CompareTo(a.width);
            else
                return b.height.CompareTo(a.height);
        });

        List<string> options = new List<string>();
        for (int i = 0; i < _filteredResolutions.Count; i++)
        {
            string resolutionOption = _filteredResolutions[i].width + "x" + _filteredResolutions[i].height + " " + _filteredResolutions[i].refreshRateRatio.value.ToString("0.##") + " Hz";
            options.Add(resolutionOption);
            if (_filteredResolutions[i].width == Screen.width && _filteredResolutions[i].height == Screen.height && (float)_filteredResolutions[i].refreshRateRatio.value == _currentRefreshRate)
            {
                _currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);

        // Ensure we have a valid index
        if (_filteredResolutions.Count > 0)
        {
            resolutionDropdown.value = _currentResolutionIndex = Mathf.Clamp(_currentResolutionIndex, 0, _filteredResolutions.Count - 1);
            resolutionDropdown.RefreshShownValue();
            SetResolution(_currentResolutionIndex);
        }
        else
        {
            Debug.LogError("No valid resolutions available to set.");
        }
    }


    public void SetResolution(int resolutionIndex)
    {
        if (resolutionIndex < 0 || resolutionIndex >= _filteredResolutions.Count)
        {
            Debug.LogError($"Invalid resolution index: {resolutionIndex}");
            return;
        }

        Resolution resolution = _filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, _isFullScreen);
    }


    public void SetWindowed(bool value)
    {
        _isFullScreen = value;
    }
}