using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIVolumeSlider : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private MixerGroup mixerGroup;
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = AudioManager.instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        audioManager.GetVolume(mixerGroup);
    }

    //function that changes the volume of a mixer group basedon slidewri input
    public void OnValueChange(float value)
    {
        audioManager.SetVolume(mixerGroup, value);
    }
    
}
