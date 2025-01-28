using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    [Header("Ingredient/Potion Parent")]
    [SerializeField] private Transform handPosition;

    // Ingredient/ Potion / Crate Variables
    private RecipeStepSO _stepInHand;
    private GameObject potionInHand;
    private GameObject ingredientStepGO;

    [Header("Holder for dropped items")]
    [SerializeField] private GameObject droppedItems;

    private IInteractable _interactablesInRange;
    private IPickupable _pickupablesInRange;

    private void OnEnable()
    {
        InputManager.instance.InteractAction += Interact;
        InputManager.instance.PickupAction += Pickup_Drop;
        Actions.OnRemovePotion += RemovePotion;
    }
    private void OnDisable()
    {
        InputManager.instance.InteractAction -= Interact;
        InputManager.instance.PickupAction -= Pickup_Drop;
        Actions.OnRemovePotion -= RemovePotion;
    }

    private void Interact(InputAction.CallbackContext input)
    {
        if(input.performed)
        {
            if (_interactablesInRange == null) return;

            _interactablesInRange.Interact(this);
        }
    }

    private void Pickup_Drop(InputAction.CallbackContext input)
    {
        if(input.performed)
        {
            if (_pickupablesInRange == null) return;

            if (handPosition.childCount == 0)
            {
                _pickupablesInRange.Pickup(this);
            }
            else
            {
                GameObject childObj = handPosition.GetChild(0).gameObject;

                if (childObj.TryGetComponent(out IPickupable pickupable))
                {
                    pickupable.Drop(droppedItems.transform);

                    if (childObj == ingredientStepGO || childObj == potionInHand)
                    {
                        if (childObj == ingredientStepGO)
                        {
                            ingredientStepGO = null;
                            _stepInHand = null;
                        }
                        else if (childObj == potionInHand)
                        {
                            potionInHand = null;
                        }
                        return;
                    }
                }
            }
        }
    }


    #region Ingredient
   // pick up ingredient from crate
   public void PickUpIngredient(GameObject ingredient)
    {
        if (ingredientStepGO != null) return;

        ingredientStepGO = ingredient;
        ingredientStepGO.GetComponent<Rigidbody>().isKinematic = true;
        ingredientStepGO.transform.SetParent(handPosition);
        ingredientStepGO.transform.position = handPosition.position;
        
        if(ingredientStepGO.TryGetComponent(out IngredientHolder ingredientHolder))
        {
            _stepInHand = ingredientHolder.recipeStepIngredient;
        }
    }

    // drop ingredient into cauldron - new parent is inside the cauldron. We'd like the ingredient to look like its jumping into the cauldron
    public void PutIngredientInCauldron(Transform newParent)
    {
        ingredientStepGO.transform.DOJump(
            endValue: newParent.position, 
            jumpPower: 1f, 
            numJumps: 1, 
            duration: 0.5f).SetEase(Ease.InOutSine);

        ingredientStepGO.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutSine);
        ingredientStepGO.transform.SetParent(null);
        ingredientStepGO.GetComponent<Rigidbody>().isKinematic = false;


        ingredientStepGO = null;
        _stepInHand = null;
    }
  
    public RecipeStepSO GetRecipeStep()
    {
        if (ingredientStepGO == null) return null;
        return _stepInHand;
    }

    internal GameObject GetGameObject()
    {
        if (ingredientStepGO == null) return null;

        return ingredientStepGO;
    }


    #endregion

    #region Potion
    public bool HasPotion() => potionInHand != null;
    public void AddPotion(GameObject potion)
    {
        potionInHand = potion;
        potionInHand.GetComponent<Rigidbody>().isKinematic = true;
    }

    public GameObject GetPotion()
    {
        if (potionInHand == null) return null;

        potionInHand.SetActive(false);
        return potionInHand;
    }

    private void RemovePotion()
    {
        if (potionInHand == null) return;

        StartCoroutine(DestroyPickup(potionInHand));
    }

    public void PickUpPotion(GameObject potion)
    {
        if (potionInHand != null) return;

        potionInHand = potion;
        potionInHand.transform.SetParent(handPosition);
        potionInHand.transform.position = handPosition.position;
        potionInHand.GetComponent<Rigidbody>().isKinematic = true;

    }
    #endregion

    public Transform GetHandPosition() => handPosition;

    private void OnTriggerEnter(Collider other)
    {
        GameObject interactableObj = other.transform.parent.gameObject;

        if (interactableObj.TryGetComponent(out IInteractable interactable))
        {
            _interactablesInRange = interactable;
        }

        if (interactableObj.TryGetComponent(out IPickupable pickUp))
        {
            _pickupablesInRange = pickUp;
        }
    }

       private void OnTriggerExit(Collider other)
    {
        GameObject interactableObj = other.transform.parent.gameObject;
     
        if (interactableObj.TryGetComponent(out IInteractable interactable))
        {
            if (_interactablesInRange == interactable)
                _interactablesInRange = null;

            //InputManager.instance.stirC.Disable();
            //InputManager.instance.stirCC.Disable();
        }

        if (interactableObj.TryGetComponent(out IPickupable pickUp))
        {
            if (_pickupablesInRange == pickUp)
                _pickupablesInRange = null;
        }

    }

    private IEnumerator DestroyPickup(GameObject pickup)
    {
        yield return new WaitForSeconds(0.3f);
        Destroy(pickup);

        if (pickup == potionInHand)
        {
            potionInHand = null;
        }
        if (pickup == ingredientStepGO)
        {
            ingredientStepGO = null;
            _stepInHand = null;
        }
    }
}
