using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAudioSounds : MonoBehaviour
{
    [SerializeField] private AudioClip selectClip;
    [SerializeField] private AudioClip pressClip;
    [SerializeField] private AudioClip increaseClip;
    [SerializeField] private AudioClip decreaseClip;

    public void PlayPressSound()
    {
        AudioManager.instance.sfxManager.PlayMenuSFX(pressClip);
    }

    public void PlaySelectSound()
    {
        AudioManager.instance.sfxManager.PlayMenuSFX(selectClip);
    }

    public void PlayIncreaseSound()
    {
        AudioManager.instance.sfxManager.PlayMenuSFX(increaseClip);
    }

    public void PlayDecreaseSound()
    {
        AudioManager.instance.sfxManager.PlayMenuSFX(decreaseClip);
    }
}
