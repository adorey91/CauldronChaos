using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{
    [Header("SFX Output")]
    [SerializeField] private List<AudioSFXPlayer> sfx_Players;

    //Function that finds the target SFX player and makes it play specified audio-clip
    public void PlaySFX(SFX_Type targetSFX, AudioClip clip, bool oneShot)
    {
        for (int i = 0; i < sfx_Players.Count; i++)
        {
            //Debug.Log(sfx_Players[i].sfx_Type);

            if (sfx_Players[i].sfx_Type == targetSFX)
            {
                //Debug.Log("SFX Type found");

                if (oneShot)
                {
                    //Debug.Log("Playing SFX");
                    sfx_Players[i].PlayOneShot(clip);
                }
                else
                {
                    sfx_Players[i].Play(clip);
                }

                return;
            }
        }
    }

    public void PlayMenuSFX(AudioClip clip)
    {
        PlaySFX(SFX_Type.UISounds, clip, true);
    }
}
