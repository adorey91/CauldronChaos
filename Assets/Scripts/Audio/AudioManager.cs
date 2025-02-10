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
    [SerializeField][Range(0.0001f, 1f)] private float masterMixerDefaultVolume = 0.5f;
    [SerializeField][Range(0.0001f, 1f)] private float sfxMixerDefaultVolume = 0.5f;
    [SerializeField][Range(0.0001f, 1f)] private float musicMixerDefaultVolume = 0.5f;

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
            //Debug.Log("New Instance Set");

            DontDestroyOnLoad(this);
        }
        else
        {
            //remove copy
            Destroy(gameObject);
            //Debug.Log("New Instance Destroyed");
        }
    }

    private void Start()
    {
        //sets default mixer volumes
        SetVolume(MixerGroup.Master, masterMixerDefaultVolume);
        SetVolume(MixerGroup.Music, musicMixerDefaultVolume);
        SetVolume(MixerGroup.SFX, sfxMixerDefaultVolume);

        //Debug.Log("MasterGroup =" + GetVolume(MixerGroup.Master));
        //Debug.Log("MusicGroup =" + GetVolume(MixerGroup.Music));
        //Debug.Log("SFXGroup =" + GetVolume(MixerGroup.SFX));
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

        //Debug.Log(value);
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
        //Debug.Log((db / 20f));
        return Mathf.Pow(10, (db / 20f));
    }
}
