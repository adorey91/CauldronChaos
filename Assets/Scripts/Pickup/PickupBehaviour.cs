using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickupBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PickupDetection pickupVolume; //the detector for picking up objects
    [SerializeField] private Transform pickupHolder; //transform holding the held location of the pickup
    [SerializeField] private InteractionBehaviour interactionBehaviour; //component containing behaviour for object interactions
    private PickupObject heldObject = null; //reference to object in hand

    //Function that runs when Gameobject script is attached to is enabled
    private void OnEnable()
    {
        InputManager.PickupAction += Pickup; //subscribing to the action for picking up
    }

    //Function that runs when Gameobject script is attached to is disabled
    private void OnDisable()
    {
        InputManager.PickupAction -= Pickup; //un-subscribing to the action for picking up
    }

    //function that handles picking up an object
    private void Pickup(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            //check if player is holding something
            if (heldObject != null)
            {
                heldObject.Drop(); //return object to normal physics
                pickupVolume.AddPickupToList(heldObject); //adds the pickup back to pickup detection list

                //check if held item is an interactable
                if (interactionBehaviour.GetHeldInteractable() != null)
                {
                    //if held interactable is detected set to null
                    interactionBehaviour.UpdateHeldInteractable(null);
                }

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

                //try to get interactable component of the held object
                Interactable interactable = heldObject.GetComponent<Interactable>();
                if (interactable != null)
                {
                    //add interactable as being held
                    interactionBehaviour.UpdateHeldInteractable(interactable);
                }
            }
        }
    }
}
