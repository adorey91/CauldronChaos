using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer masterMixer;

    [Header("Audio Sliders")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider masterSlider;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Background Clips")]
    [SerializeField] private AudioClip mainMenuClip;
    [SerializeField] private AudioClip gameplayClip;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip temp;

    private void Awake()
    {
        SetVolume();
    }

    private void SetVolume()
    {
        OnChangeSlider("SFX");
        OnChangeSlider("Music");
        OnChangeSlider("Master");
    }

    public void OnChangeSlider(string slider)
    {
        switch (slider)
        {
            case "SFX": masterMixer.SetFloat(slider, Mathf.Log10(sfxSlider.value) * 20); break;
            case "Music": masterMixer.SetFloat(slider, Mathf.Log10(musicSlider.value) * 20); break;
            case "Master": masterMixer.SetFloat(slider, Mathf.Log10(masterSlider.value) * 20); break;
            default: Debug.Log("No mixer named " + slider); break;
        }
    }
}