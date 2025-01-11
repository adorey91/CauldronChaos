using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionDetector : MonoBehaviour
{
    [Header("Ingredient/Potion Parent")]
    [SerializeField] private Transform handPosition;

    // Ingredient/ Potion / Crate Variables
    private IngredientSO ingredientInHand;
    private GameObject potionInHand;
    private GameObject ingredientGO;
    private GameObject _crateObject;

    private IInteractable _interactablesInRange;

    private void OnEnable()
    {
        Actions.OnInteract += Interact;
        Actions.OnRemovePotion += RemovePotion;
        Actions.OnRemoveIngredient += RemoveIngredient;
    }
    private void OnDisable()
    {
        Actions.OnInteract -= Interact;
        Actions.OnRemovePotion -= RemovePotion;
        Actions.OnRemoveIngredient -= RemoveIngredient;
    }

    private void Interact()
    {
        if (_interactablesInRange == null) return;

        _interactablesInRange.Interact(this);
    }

    #region Ingredient
    public bool HasIngredient() => ingredientGO != null;

    public void AddIngredient(GameObject ingredient)
    {
        ingredientGO = Instantiate(ingredient, handPosition);
        ingredientGO.GetComponent<Rigidbody>().isKinematic = true;
        ingredientInHand = ingredientGO.GetComponent<IngredientHolder>().ingredient;

    }

    public IngredientSO GetIngredient()
    {
        if (ingredientInHand == null) return null;

        ingredientGO.SetActive(false);
        return ingredientInHand;
    }

    private void RemoveIngredient()
    {
        if(ingredientGO == null) return;

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
        if(potionInHand == null) return;

        StartCoroutine(DestroyPickup(potionInHand));
    }    
    #endregion

    public Transform GetHandPosition() => handPosition;

    private void OnTriggerEnter(Collider other)
    {
        GameObject interactableObj = other.transform.parent.gameObject;
        IInteractable interactable = interactableObj.GetComponent<IInteractable>();
     
        if(interactable == null) return;

        _interactablesInRange = interactable;
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject interactableObj = other.transform.parent.gameObject;
        IInteractable interactable = interactableObj.GetComponent<IInteractable>();
        
        if(interactable == null) return;
        
        if (_interactablesInRange == interactable)
            _interactablesInRange = null;
    }

    private IEnumerator DestroyPickup(GameObject pickup)
    {
        yield return new WaitForSeconds(0.3f);
        Destroy(pickup);
    }
}
