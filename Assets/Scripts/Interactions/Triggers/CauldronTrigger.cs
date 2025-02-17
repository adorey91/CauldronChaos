using UnityEngine;
using System;

public class CauldronTrigger : MonoBehaviour
{
    [SerializeField] private CauldronInteraction cauldron; //cached referene to the cauldron

    //Function called whenever a collider enters the trigger volume
    private void OnTriggerStay(Collider other)
    {
        //try to get ingredient component of collider
        PickupObject ingredientHolder = other.GetComponent<PickupObject>();
        if (ingredientHolder != null && !ingredientHolder.AddedToCauldron() && !ingredientHolder.isHeld)
        {
            ingredientHolder.GetComponent<Rigidbody>().isKinematic = true;
            ingredientHolder.AddToCauldron();
            cauldron.AddIngredient(ingredientHolder, ingredientHolder.gameObject);
        }
    }
}
