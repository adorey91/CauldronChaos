using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class DoorInteraction : MonoBehaviour
{
    [Header("SFX")]
    [SerializeField] private SFXLibrary newCustomerSFX;
    [SerializeField] private GameObject door;
    private int customerCount;

    private void OpenDoor(bool isLeft)
    {
        //playing SFX for new customer arriving
        //Debug.Log("Opening Door");
        if (isLeft)
        {
            AudioManager.instance.sfxManager.PlaySFX(SFX_Type.ShopSounds, newCustomerSFX.PickAudioClip(), true);
        }

        door.transform.DOKill();
        door.transform.DOLocalRotate(new Vector3(0, 90, 0), 1f);
    }

    private void CloseDoor()
    {
        //Debug.Log("Closing Door");
        door.transform.DOKill();
        door.transform.DOLocalRotate(new Vector3(0, 0, 0), 1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Customer"))
        {
            customerCount++;
            if (this.gameObject.name == "DoorInTrigger")
            {
                OpenDoor(true);
            }
            else
            {
                OpenDoor(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Customer"))
        {
            customerCount--; // Decrease count when a customer exits
            customerCount = Mathf.Max(customerCount, 0); // Ensure it never goes below 0

            if (customerCount == 0) // Close door only if no customers are left
            {
                CloseDoor();
            }
        }
    }
}
