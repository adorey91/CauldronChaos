using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractionDetector : MonoBehaviour
{
    [Header("Ingredient/Potion Parent")]
    [SerializeField] private Transform handPosition;

    // Ingredient/ Potion / Crate Variables
    private IngredientSO ingredientInHand;
    private GameObject potionInHand;
    private GameObject ingredientGO;

    [Header("Holder for dropped items")]
    [SerializeField] private GameObject droppedItems;

    private IInteractable _interactablesInRange;
    private IPickupable _pickupablesInRange;

    private void OnEnable()
    {
        Actions.OnInteract += Interact;
        Actions.OnPickup += Pickup_Drop;
        Actions.OnRemovePotion += RemovePotion;
        Actions.OnRemoveIngredient += RemoveIngredient;
    }
    private void OnDisable()
    {
        Actions.OnInteract -= Interact;
        Actions.OnPickup -= Pickup_Drop;
        Actions.OnRemovePotion -= RemovePotion;
        Actions.OnRemoveIngredient -= RemoveIngredient;
    }

    private void Interact()
    {
        if (_interactablesInRange == null) return;

        _interactablesInRange.Interact(this);
    }

    private void Pickup_Drop()
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

                if(childObj == ingredientGO || childObj == potionInHand)
                {
                    if(childObj == ingredientGO)
                    {
                        ingredientGO = null;
                        ingredientInHand = null;
                    }
                    else if(childObj == potionInHand)
                    {
                        potionInHand = null;
                    }
                    return;
                }
            }
        }
    }


    #region Ingredient
    public bool HasIngredient()
    {
        if (ingredientGO == null)
        {
            return false;
        }
        return true;
    }


    /// <summary>
    /// Add ingredient to hand position from crate
    /// </summary>
    /// <param name="ingredient"></param>
    public void AddIngredient(GameObject ingredient)
    {
        ingredientGO = Instantiate(ingredient, handPosition);
        ingredientGO.GetComponent<Rigidbody>().isKinematic = true;
        ingredientInHand = ingredientGO.GetComponent<IngredientHolder>().ingredient;

    }

    /// <summary>
    /// Picks up ingredient and sets it to hand position
    /// </summary>
    /// <param name="ingredient"></param>
    public void PickUpIngredient(GameObject ingredient)
    {
        ingredientGO = ingredient;
        ingredientGO.transform.SetParent(handPosition);
        ingredient.transform.position = handPosition.position;
        ingredientGO.GetComponent<Rigidbody>().isKinematic = true;
        ingredientInHand = ingredientGO.GetComponent<IngredientHolder>().ingredient;
    }

    /// <summary>
    /// Returns IngredientSO from hand, sets gameobject to inactive
    /// </summary>
    /// <returns></returns>
    public IngredientSO GetIngredient()
    {
        if (ingredientInHand == null) return null;

        ingredientGO.SetActive(false);

        return ingredientInHand;
    }

    /// <summary>
    /// Removes ingredient from scene after a delay
    /// </summary>
    private void RemoveIngredient()
    {
        if (ingredientGO == null) return;

        StartCoroutine(DestroyPickup(ingredientGO));
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
            _interactablesInRange = interactable;

        if (interactableObj.TryGetComponent(out IPickupable pickUp))
            _pickupablesInRange = pickUp;
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject interactableObj = other.transform.parent.gameObject;

        if (interactableObj.TryGetComponent(out IInteractable interactable))
        {
            if (_interactablesInRange == interactable)
                _interactablesInRange = null;
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

        if(pickup == potionInHand)
        {
            potionInHand = null;
        }
        if(pickup == ingredientGO)
        {
            ingredientGO = null;
            ingredientInHand = null;
        }
    }
}
