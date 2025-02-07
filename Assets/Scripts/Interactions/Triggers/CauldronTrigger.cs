using UnityEngine;
using System;

public class CauldronTrigger : MonoBehaviour
{
    [SerializeField] private CauldronInteraction cauldron; //cached referene to the cauldron
    Transform cauldronTransform;

    public static Action addedItem;

    private void Start()
    {
        cauldronTransform = transform.parent;
    }

    //Function called whenever a collider enters the trigger volume
    private void OnTriggerEnter(Collider other)
    {
        //try to get ingredient component of collider
        IngredientHolder ingredientHolder = other.GetComponent<IngredientHolder>();
        if (ingredientHolder != null && !ingredientHolder.AddedToCauldron())
        {
            //calls cauldron functionality for adding an ingredient
            //other.gameObject.GetComponent<PickupObject>().Drop();
            addedItem?.Invoke();

            ingredientHolder.GetComponent<Rigidbody>().isKinematic = true;
            ingredientHolder.AddToCauldron();
            GameObject ingredient = ingredientHolder.gameObject;
            ingredient.transform.SetParent(cauldronTransform);

            cauldron.AddIngredient(ingredientHolder, ingredientHolder.gameObject);
        }
    }
}
