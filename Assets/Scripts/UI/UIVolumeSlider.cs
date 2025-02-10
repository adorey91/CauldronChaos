using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIVolumeSlider : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private MixerGroup mixerGroup;

    // Start is called before the first frame update
    void Start()
    {
        volumeSlider.value = AudioManager.instance.GetVolume(mixerGroup);
    }

    //function that changes the volume of a mixer group basedon slidewri input
    public void OnValueChange(float value)
    {
        if (value < 0.0001f)
        {
            value = 0.0001f;
        }

        AudioManager.instance.SetVolume(mixerGroup, value);
    }
    
}
