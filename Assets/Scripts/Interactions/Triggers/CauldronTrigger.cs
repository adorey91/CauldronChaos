using UnityEngine;
using System;

public class CauldronTrigger : MonoBehaviour
{
    [SerializeField] private CauldronInteraction cauldron; //cached referene to the cauldron
    [SerializeField] private Transform cauldronItems;

    public static Action addedItem;


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
            cauldron.AddIngredient(ingredientHolder, ingredientHolder.gameObject);
        }
    }
}
