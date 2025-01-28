using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum MixerGroup
{
    Master,
    Music,
    SFX
}

public class AudioManager : MonoBehaviour
{
    [Header("Audio Managers")]
    public SFXManager sfxManager;
    public MusicManager musicManager;

    [Header("Audio Mixers")]
    [SerializeField] private AudioMixer masterMixer;

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
                _instance.name = "AudioManager"; //renames the game object to InputManager
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

    //function that returns the current volume of target mixer group
    public float GetVolume(MixerGroup target)
    {
        float value = 0f;

        switch (target)
        {
            case MixerGroup.Master:
                masterMixer.GetFloat("MasterVolume", out value);
                break;

            case MixerGroup.Music:
                masterMixer.GetFloat("MusicVolume", out value);
                break;

            case MixerGroup.SFX:
                masterMixer.GetFloat("SFXVolume", out value);
                break;
        }

        value = ConvertFromDB(value);
        return value;
    }

    //utility function for converting linear float to DB
    private float ConvertToDB(float value)
    {
        return Mathf.Log10(value) * 20f;
    }

    //utility function for converting DB to a linear float
    private float ConvertFromDB(float db)
    {
        return Mathf.Pow(10, (db / 20f));
    }
}
