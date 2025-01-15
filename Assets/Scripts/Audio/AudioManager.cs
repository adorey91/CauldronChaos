using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public enum MixerGroup
    {
        Master,
        Music,
        SFX
    }

    [Header("Audio Managers")]
    public SFXManager sfxManager;
    public MusicManager musicManager;

    [Header("Audio Mixers")]
    [SerializeField] private AudioMixer masterMixer;

    [Header("UI Elements")]
    [SerializeField] private Slider[] masterSliders;
    [SerializeField] private Slider[] musicSliders;
    [SerializeField] private Slider[] sfxSliders;

    [Header("Volume Settings")]
    [SerializeField] private float initialVolume = 0.5f;

    private static AudioManager _instance;

    //function that checks if instance exists and spawns one if it does not
    public static AudioManager instance
    {
        get
        {
            //check if instance is null
            if (_instance == null)
            {
                //spawn instance
                _instance = Instantiate(Resources.Load("AudioManager") as GameObject).GetComponent<AudioManager>();
                _instance.name = "InputManager"; //renames the game object to InputManager
            }
            return _instance; //returns 
        }
    }

    // Awake is called before the first frame update and before start
    void Awake()
    {
        //check if this is the active instance
        if (!_instance || _instance == this)
        {
            _instance = this;
        }
        else
        {
            //remove copy
            Destroy(gameObject);
        }

        // This isnt needed as it's now nested under the game manager.
        //DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        /*
        InitializeMixer(MixerGroup.Master);
        InitializeMixer(MixerGroup.Music);
        InitializeMixer(MixerGroup.SFX);
        */
    }

    //function that sets volume of mixers
    public void SetVolume(MixerGroup target, float volume)
    {
        switch (target)
        {
            case MixerGroup.Master:
                masterMixer.SetFloat("MasterVolume", ConvertToDB(volume)); //replace string with 
                break;

            case MixerGroup.Music:
                masterMixer.SetFloat("MusicVolume", ConvertToDB(volume));
                break;

            case MixerGroup.SFX:
                masterMixer.SetFloat("SFXVolume", ConvertToDB(volume));
                break;
        }
    }

    public void SetMasterVolume(float sliderValue)
    {
        SetVolume(MixerGroup.Master, sliderValue);
    }

    public void SetSFXVolume(float sliderValue)
    {
        SetVolume(MixerGroup.SFX, sliderValue);
    }

    public void SetMusicVolume(float sliderValue)
    {
        SetVolume(MixerGroup.Music, sliderValue);
    }

    //utility function for converting linear float to DB
    private float ConvertToDB(float value)
    {
        return Mathf.Log10(value) * 20f;
    }

    private void InitializeMixer(MixerGroup mixerGroup)
    {
        switch (mixerGroup)
        {
            case MixerGroup.Master:
                masterMixer.SetFloat("MasterVolume", ConvertToDB(initialVolume));

                //setting master sliders to initial volume
                for (int i = 0; i < masterSliders.Length; i++)
                {
                    masterSliders[i].value = initialVolume;
                }
                break;

            case MixerGroup.Music:
                masterMixer.SetFloat("MusicVolume", ConvertToDB(initialVolume));

                //setting music sliders to initial volume
                for (int i = 0; i < musicSliders.Length; i++)
                {
                    musicSliders[i].value = initialVolume;
                }
                break;

            case MixerGroup.SFX:
                masterMixer.SetFloat("SFXVolume", ConvertToDB(initialVolume));

                //setting sfx sliders to initial volume
                for (int i = 0; i < sfxSliders.Length; i++)
                {
                    sfxSliders[i].value = initialVolume;
                }
                break;
        }
    }
}
