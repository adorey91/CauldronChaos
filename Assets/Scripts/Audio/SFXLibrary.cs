using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SFXLibrary : MonoBehaviour
{
    [SerializeField] private AudioClip[] audioClips;
    private AudioClip _lastPlayed = null;

    //function that picks an audioclip from the list of audio clips for the SFX player
    public AudioClip PickAudioClip()
    {
        //clause statement for if the player has less than 1 audioclips
        if (audioClips.Length <= 0) return null;

        AudioClip audioClip = null;

        //if more than one audio clip pick one that wasn't last played
        if (audioClips.Length > 1)
        {
            //pick random audio clip
            audioClip = audioClips[Random.Range(0, audioClips.Length)];

            //if audio picked is the last one played pick a different one
            while (audioClip == _lastPlayed)
            {
                audioClip = audioClips[Random.Range(0, audioClips.Length)];
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
