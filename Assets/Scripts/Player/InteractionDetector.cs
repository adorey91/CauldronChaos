using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    [Header("Ingredient/Potion Parent")]
    [SerializeField] private Transform handPosition;

    // Ingredient/ Potion / Crate Variables
    private GameObject potionInHand; // used to store the potion in hand
    private GameObject ingredientStepGO; // used to store the ingredient in hand - GameObject to check if there is an ingredient in hand
    [SerializeField] private Image currentPickup; // used to show the current pickup in the UI

    [Header("Holder for dropped items")]
    [SerializeField] private GameObject droppedItems; // used to store dropped items for later deleting.

    private IInteractable _interactablesInRange;
    private IPickupable _pickupablesInRange;


    public void Start()
    {
        currentPickup.enabled = false;
    }

    private void OnEnable()
    {
        InputManager.InteractAction += Interact;
        InputManager.PickupAction += Pickup_Drop;
        Actions.OnRemovePotion += RemovePotion;
    }
    private void OnDisable()
    {
        InputManager.InteractAction -= Interact;
        InputManager.PickupAction -= Pickup_Drop;
        Actions.OnRemovePotion -= RemovePotion;
    }

    private void Interact(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            if (_interactablesInRange == null) return;

            _interactablesInRange.Interact(this);
        }
    }

    private void Pickup_Drop(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            // if there is no pickupable in range, return
            if (_pickupablesInRange == null) return;

            // if there is no item in hand, pick up the item
            if (handPosition.childCount == 0)
            {
                _pickupablesInRange.Pickup(this);

                currentPickup.enabled = true;
            }
            // if there is an item in hand, drop the item
            else
            {

                GameObject childObj = handPosition.GetChild(0).gameObject;

                // if the item in hand is a pickupable, drop the item - potions and ingredients should have this interface
                if (childObj.TryGetComponent(out IPickupable pickupable))
                {
                    pickupable.Drop(droppedItems.transform);

                    ingredientStepGO = null;
                    potionInHand = null;

                    currentPickup.enabled = false;
                    return;
                }
            }
        }
    }


    #region Ingredient
    // Checks for ingredient
    internal bool HasIngredient => ingredientStepGO != null;

    // pick up ingredient from crate
    public void PickUpIngredient(GameObject ingredient)
    {
        // if there is an ingredient in hand, return
        if (ingredientStepGO != null) return;

        ingredientStepGO = ingredient;
        ingredientStepGO.GetComponent<Rigidbody>().isKinematic = true;
        ingredientStepGO.transform.SetParent(handPosition);
        ingredientStepGO.transform.position = handPosition.position;

        // get the recipe step from the ingredient and set the current pickup sprite to the ingredient sprite
        if (ingredientStepGO.TryGetComponent(out IngredientHolder ingredientHolder))
        {
            RecipeStepSO step = ingredientHolder.recipeStepIngredient;
            currentPickup.enabled = true;
            currentPickup.sprite = step.ingredientSprite;
        }
    }

    // drop ingredient into cauldron - new parent is inside the cauldron. We'd like the ingredient to look like its jumping into the cauldron
    public void PutIngredientInCauldron(Transform newParent)
    {
        // if there is no ingredient in hand, return
        if (ingredientStepGO == null) return;

        // ingredient jumps into the cauldron, slowly gets small and the parent is removed
        ingredientStepGO.transform.DOJump(newParent.position, 1f, 1, 0.5f).SetEase(Ease.InOutSine);
        ingredientStepGO.transform.DOScale(Vector3.zero, 1f).SetEase(Ease.InOutSine);
        ingredientStepGO.transform.SetParent(null);
        ingredientStepGO.GetComponent<Rigidbody>().isKinematic = false;

        ingredientStepGO = null;
        currentPickup.enabled = false;
    }


    internal GameObject GetIngredientObject()
    {
        if (ingredientStepGO == null) return null;

        return ingredientStepGO;
    }
    #endregion

    #region Potion
    // Checks if has potion
    internal bool HasPotion => potionInHand != null;

    // Returns potion if the player has one
    public GameObject GetPotion()
    {
        if (potionInHand == null) return null;

        potionInHand.SetActive(false);
        return potionInHand;
    }

    // I think theres a better way to deal with this but the potion in hand needs to be removed/ destroyed after given to a customer
    private void RemovePotion()
    {
        if (potionInHand == null) return;

        StartCoroutine(DestroyPickup(potionInHand));
    }

    // Is used to pick up potion, sets parent to hand.
    public void PickUpPotion(GameObject potion)
    {
        if (potionInHand != null) return;

        potionInHand = potion;
        potionInHand.transform.SetParent(handPosition);
        potionInHand.transform.position = handPosition.position;
        potionInHand.GetComponent<Rigidbody>().isKinematic = true;
        currentPickup.sprite = potionInHand.GetComponent<PotionOutput>().potionInside.potionIcon;

    }
    #endregion

    // Used to check if an interactable or pickup is in range.
    private void OnTriggerStay(Collider other)
    {
        GameObject interactableObj = other.transform.parent.gameObject;

        if (interactableObj.TryGetComponent(out IInteractable interactable))
            _interactablesInRange = interactable;

        if (interactableObj.TryGetComponent(out IPickupable pickUp))
            _pickupablesInRange = pickUp;
    }

    // Checks to see what trigger is exit and then it removes whichever that was.
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
        currentPickup.enabled = false;
        if (pickup == potionInHand)
            potionInHand = null;
        if (pickup == ingredientStepGO)
            ingredientStepGO = null;
    }
}
