using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SFX_Type
{
    Pickup,
    Drop,
    UISounds
}

[System.Serializable]
public class AudioSFXPlayer : MonoBehaviour
{
    [SerializeField] public SFX_Type sfx_Type;
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public float pitchVariance;
    [SerializeField] private float cooldownTime;
    [SerializeField] private AudioClip[] audioClips;
    private bool played;
    private AudioClip lastPlayed = null;

    //Function to call audio SFX that does not inturrupt playing clip (can overlap)
    public void PlayOneShot()
    {
        if (!played)
        {
            audioSource.pitch = 1 + Random.Range(-pitchVariance, pitchVariance);
            audioSource.PlayOneShot(PickAudioClip());
            played = true;

            StartCoroutine(AudioCooldown());
        }
    }

    //Function to call audio SFX that inturrupts playing clip
    public void Play()
    {
        audioSource.pitch = 1 + Random.Range(-pitchVariance, pitchVariance);
        audioSource.clip = PickAudioClip();
        audioSource.Play();
    }

    //Coroutine to put audio on cooldown
    private IEnumerator AudioCooldown()
    {
        yield return new WaitForSeconds(cooldownTime);
        played = false;
    }

    //function that picks an audioclip from the list of audio clips for the SFX player
    private AudioClip PickAudioClip()
    {
        //clause statement for if the player has less than 1 audioclips
        if (audioClips.Length <= 0) return null;

        AudioClip audioClip = null;

        //if more than one audio clip pick one that wasn't last played
        if (audioClips.Length > 1)
        {
            //pick random audio clip
            audioClip = audioClips[Random.Range(0, audioClips.Length -1)];

            //if audio picked is the last one played pick a different one
            while (audioClip == lastPlayed)
            {
                audioClip = audioClips[Random.Range(0, audioClips.Length - 1)];
            }
        }
        //pick only audioclip
        else
        {
            audioClip = audioClips[0];
        }

        return audioClip;
    }
}
