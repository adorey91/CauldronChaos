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
    private CustomTimer imageTimer;
    private float _imageTime = 3f;
    [SerializeField] private GameObject backButton;
    private bool pressedButton;

    public static Action<bool> OnActivateHowToPlay;
    public static Action OnDeactivateHowToPlay;


    private void Awake()
    {
        howToPlayCanvas = GetComponent<Canvas>();
        imageTimer = new CustomTimer(_imageTime, false);
    }

    private void Start()
    {
        if (_howToPlayImages == null || _howToPlayImages.Length == 0)
        {
            Debug.LogError("Error: _howToPlayImages is null or empty!");
            return;
        }
    }

    private void OnEnable()
    {
        OnActivateHowToPlay += ActivateHowToPlay;
        OnDeactivateHowToPlay += DeactivateHowToPlay;
    }

    private void OnDisable()
    {
        OnActivateHowToPlay -= ActivateHowToPlay;
        OnDeactivateHowToPlay -= DeactivateHowToPlay;
    }

    private void Update()
    {
        if (imageTimer.UpdateTimer() && !pressedButton)
        {
            NextImage();
        }
    }

    // Activates the HowToPlay canvas
    public void ActivateHowToPlay(bool isLoading)
    {
        // Sets the first image in the array
        pressedButton = false;
        howToPlayCanvas.enabled = true;
        ControllerSelect.OnFirstSelect?.Invoke("HowToPlay");
        howToPlayImage.sprite = _howToPlayImages[0];

        if(isLoading)
        {
            imageBG.enabled = false;
            backButton.SetActive(false);
        }
        else
        {
            imageBG.enabled = true;
            backButton.SetActive(true);
        }
        
        imageTimer.StartTimer();
    }

    public void DeactivateHowToPlay()
    {
        howToPlayCanvas.enabled = false;
        imageTimer = new CustomTimer(_imageTime, false);
    }

    public void NextImage()
    {
        int currentIndex = System.Array.IndexOf(_howToPlayImages, howToPlayImage.sprite);

        // Move to next image, or loop back to the first if at the end
        int nextIndex = (currentIndex + 1) % _howToPlayImages.Length;
        howToPlayImage.sprite = _howToPlayImages[nextIndex];

        imageTimer.ResetTimer();
    }

    public void PreviousImage()
    {
        int currentIndex = System.Array.IndexOf(_howToPlayImages, howToPlayImage.sprite);

        // Move to previous image, or loop back to the last if at the beginning
        int prevIndex = (currentIndex - 1 + _howToPlayImages.Length) % _howToPlayImages.Length;
        howToPlayImage.sprite = _howToPlayImages[prevIndex];

        imageTimer.ResetTimer();
    }

    public void PressedButton()
    {
        pressedButton = true;
    }
}
