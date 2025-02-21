using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SFX_Type
{
    PlayerSounds,
    ShopSounds,
    StationSounds,
    ItemInteraction,
    UISounds,
    GoblinSounds,
    ConstantSounds
}

[System.Serializable]
public class AudioSFXPlayer : MonoBehaviour
{
    [SerializeField] public SFX_Type sfx_Type;
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public float pitchVariance;
    [SerializeField] private float cooldownTime;
    [SerializeField] private float fadeTime;
    private bool played;  

    //Function to call audio SFX that does not inturrupt playing clip (can overlap)
    public void PlayOneShot(AudioClip audioClip)
    {
        if (!played)
        {
            audioSource.pitch = 1 + Random.Range(-pitchVariance, pitchVariance);
            audioSource.PlayOneShot(audioClip);
            played = true;

            StartCoroutine(AudioCooldown());
        }
    }

    //Function to call audio SFX that inturrupts playing clip
    public void Play(AudioClip audioClip)
    {
        audioSource.pitch = 1 + Random.Range(-pitchVariance, pitchVariance);
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    //Coroutine to put audio on cooldown
    private IEnumerator AudioCooldown()
    {
        yield return new WaitForSeconds(cooldownTime);
        played = false;
    }
}
