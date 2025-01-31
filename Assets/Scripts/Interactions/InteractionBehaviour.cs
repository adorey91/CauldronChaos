using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InteractionDetection interactionVolume;
    private Interactable heldInteractable = null;

   //Function that runs when Gameobject script is attached to is enabled
    private void OnEnable()
    {
        InputManager.InteractAction += TryInteract; //subscribing to the action for interacting
    }

    //Function that runs when Gameobject script is attached to is disabled
    private void OnDisable()
    {
        InputManager.InteractAction -= TryInteract; //un-subscribing to the action for interacting
    }

    //Function that tries to use an interactable
    private void TryInteract(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            Debug.Log("Interacting");
            //use held interactable if available
            if (heldInteractable != null)
            {
                heldInteractable.Interact();
            }
            else
            {
                //try to get a non-held interactable from the interaction volume
                Interactable interactable;
                interactable = interactionVolume.GetFirstNonHeldInteractable();

                //check if was able to find a non-held interactable
                if (interactable != null)
                {
                    interactable.Interact();
                }
                else
                {
                    //any interaction failed effects could go here
                }
            }
        }
    }

    //Mutator method that updates the held interactable
    public void UpdateHeldInteractable(Interactable heldInteractable)
    {
        this.heldInteractable = heldInteractable;
    }

    //Accesssor method that returns the held interactable
    public Interactable GetHeldInteractable()
    {
        return heldInteractable;
    }
}
