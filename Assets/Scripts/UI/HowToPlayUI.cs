using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HowToPlayUI : MonoBehaviour
{
    [SerializeField] private Canvas howToPlayCanvas;
    [SerializeField] private Sprite[] _howToPlayImages;
    [SerializeField] private Image howToPlayImage;
    [SerializeField] private Image imageBG;
    [SerializeField] private GameObject backButton;

    private void Awake()
    {
        howToPlayCanvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        if (_howToPlayImages == null || _howToPlayImages.Length == 0)
        {
            Debug.LogError("Error: _howToPlayImages is null or empty!");
            return;
        }
    }

    #region OnEnable / OnDisable / OnDestroy Events
    private void OnEnable()
    {
        Actions.OnActivateHowToPlay += ActivateHowToPlay;
        Actions.OnDeactivateHowToPlay += DeactivateHowToPlay;
    }

    private void OnDisable()
    {
        Actions.OnActivateHowToPlay -= ActivateHowToPlay;
        Actions.OnDeactivateHowToPlay -= DeactivateHowToPlay;
    }

    private void OnDestroy()
    {
        Actions.OnActivateHowToPlay -= ActivateHowToPlay;
        Actions.OnDeactivateHowToPlay -= DeactivateHowToPlay;
    }
    #endregion


    // Activates the HowToPlay canvas
    public void ActivateHowToPlay(bool isLoading)
    {
        // Sets the first image in the array
        howToPlayCanvas.enabled = true;

        if(isLoading)
        {
            imageBG.enabled = false;
            backButton.SetActive(false);
        }
        else
        {
            imageBG.enabled = true;
            Actions.OnFirstSelect?.Invoke("HowToPlay");
            backButton.SetActive(true);
        }
        
    }

    public void DeactivateHowToPlay()
    {
        howToPlayCanvas.enabled = false;
    }
}
