using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DoorInteraction : MonoBehaviour
{
    [Header("SFX")]
    [SerializeField] private SFXLibrary newCustomerSFX;

    private void OpenDoor()
    {
        //playing SFX for new customer arriving
        //Debug.Log("Opening Door");
        AudioManager.instance.sfxManager.PlaySFX(SFX_Type.ShopSounds, newCustomerSFX.PickAudioClip(), true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Customer"))
            OpenDoor();
    }
}
