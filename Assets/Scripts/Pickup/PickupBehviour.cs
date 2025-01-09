using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickupBehviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PickupDetection pickupVolume;
    [SerializeField] private Transform pickupHolder;
    private PickupObject heldObject = null;

    //input requirements
    private PlayerInputActions playerControls;
    private InputAction pickup;

    private void Awake()
    {
        playerControls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        pickup = playerControls.Player.Pickup;
        pickup.Enable();
    }

    private void OnDisable()
    {
        pickup.Disable();
    }

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

    private void PickUpObject(InputAction.CallbackContext input)
    {

    }
}
