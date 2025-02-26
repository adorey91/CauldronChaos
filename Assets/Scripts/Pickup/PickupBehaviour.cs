using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PickupBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PickupDetection pickupVolume; //the detector for picking up objects
    [SerializeField] private InteractionDetection interactionVolume; //the detector for interacting with objects
    [SerializeField] private Transform pickupHolder; //transform holding the held location of the pickup
    [SerializeField] private InteractionBehaviour interactionBehaviour; //component containing behaviour for object interactions
    private PickupObject heldObject = null; //reference to object in hand
    [SerializeField] private Animator playerAnimator;

    [Header("UI")]
    [SerializeField] private Image pickupUIHolder;
    internal bool isHoldingItem = false;

    private void Awake()
    {
        if(pickupUIHolder != null)
            pickupUIHolder.enabled = false;
    }

    #region OnEnable / OnDisable / OnDestroy Events
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

    private void OnDestroy()
    {
        InputManager.PickupAction -= Pickup;
    }

    #endregion


    //function that handles picking up an object
    private void Pickup(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            //Debug.Log("Input Activated");

            //player is holding something
            if (heldObject != null)
            {
               DropItem();
                return;
            }

            //player is not holding anything and is by an ingredient crate
            Interactable container = interactionVolume.GetContainer();
            if (container != null)
            {
                container.Interact(this);
                playerAnimator.SetTrigger("Pickup");
            }
            //pick-up off the ground
            else
            {
                //try to get held item from pickup detector
                heldObject = pickupVolume.GetPickup();

                //if item is in detection range
                if (heldObject != null)
                {
                    playerAnimator.SetTrigger("Pickup");
                    heldObject.PickUp(pickupHolder);
                    pickupUIHolder.enabled = true;

                    if (heldObject.TryGetComponent(out PotionOutput potionOutput) && potionOutput.potionInside != null)
                    {
                        pickupUIHolder.sprite = potionOutput.potionInside.potionIcon;
                    }
                    else if (heldObject.TryGetComponent(out PickupObject ingredientHolder))
                    {
                        pickupUIHolder.sprite = ingredientHolder.recipeIngredient.ingredientSprite;
                    }

                    //try to get interactable component of the held object
                    if (heldObject.TryGetComponent<Interactable>(out var interactable))
                    {
                        //add interactable as being held
                        interactionBehaviour.UpdateHeldInteractable(interactable);
                    }
                }
            }
        }
    }

    //Mutator method that manually sets the held object
    public void SetHeldObject(PickupObject targetObject)
    {
        //Debug.Log("In set held object");
        heldObject = targetObject;
        pickupVolume.RemovePickupFromList(heldObject);
        pickupUIHolder.enabled = true;
        pickupUIHolder.sprite = heldObject.GetComponent<PickupObject>().recipeIngredient.ingredientSprite;
        heldObject.PickUp(pickupHolder);
        isHoldingItem = true;
    }

    private void DropItem()
    {
        playerAnimator.SetTrigger("Drop");
        pickupUIHolder.enabled = false;
        heldObject.Drop();
        isHoldingItem = false;

        // check if held item is an interactable
        if (interactionBehaviour.GetHeldInteractable() != null)
        {
            //if held interactable is detected set to null
            interactionBehaviour.UpdateHeldInteractable(null);
        }

        heldObject = null;
    }

    public Transform GetHolderLocation()
    {
        return pickupHolder;
    }
}
