using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBehviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PickupDetection pickupVolume;
    [SerializeField] private Transform pickupHolder;
    private PickupObject heldObject = null;

    // Update is called once per frame
    void Update()
    {
        //skip if no input was received
        if (!InputManager.instance.GetPickupInput()) return;

        Debug.Log("Pickup input detected");

        //check if player is holding something
        if (heldObject != null)
        {
            heldObject.Drop();
            pickupVolume.AddPickupToList(heldObject);
            heldObject = null;
        }
        else
        {
            //try to get held item from pickup detector
            heldObject = pickupVolume.GetPickup();

            //if item is in detection range
            if (heldObject != null)
            {
                heldObject.PickUp(pickupHolder);
            }
        }
    }

}
