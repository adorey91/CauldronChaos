using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickupBehviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PickupDetection pickupVolume;
    [SerializeField] private Transform pickupHolder;
    private PickupObject heldObject = null;

    private void OnEnable()
    {
        InputManager.instance.PickupAction += Pickup;
    }

    private void OnDisable()
    {
        InputManager.instance.PickupAction -= Pickup;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Pickup(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
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
}
