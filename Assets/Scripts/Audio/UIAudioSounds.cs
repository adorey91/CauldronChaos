using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAudioSounds : MonoBehaviour
{
    [SerializeField] private AudioClip selectClip;
    [SerializeField] private AudioClip pressClip;

    public void PlayPressSound()
    {
        AudioManager.instance.sfxManager.PlayMenuSFX(pressClip);
    }

    public void PlaySelectSound()
    {
        AudioManager.instance.sfxManager.PlayMenuSFX(selectClip);
    }
}
