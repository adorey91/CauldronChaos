using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SFX_Type
{
    PlayerHit,
    PlayerShield,
    PlayerShoot,
    EnemyHit,
    EnemyShield,
    EnemyShoot,
    Explosion,
    UISounds
}

[System.Serializable]
public class AudioSFXPlayer : MonoBehaviour
{
    public SFX_Type sfx_Type;
    public AudioSource audioSource;
    public float pitchVariance;
    public float cooldownTime;
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
        audioSource.clip = audioClip;
        audioSource.pitch = 1 + Random.Range(-pitchVariance, pitchVariance);
        audioSource.Play();
    }

    //Coroutine to put audio on cooldown
    private IEnumerator AudioCooldown()
    {
        yield return new WaitForSeconds(cooldownTime);
        played = false;
    }
}
