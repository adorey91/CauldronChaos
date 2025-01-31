using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CauldronTrigger : MonoBehaviour
{
    [SerializeField] private CauldronInteraction cauldron; //cached referene to the cauldron

    //Function called whenever a collider enters the trigger volume
    private void OnTriggerEnter(Collider other)
    {
        //try to get ingredient component of collider
        IngredientHolder ingredientHolder = other.GetComponent<IngredientHolder>();
        if (ingredientHolder != null)
        {
            //calls cauldron functionality for adding an ingredient
            cauldron.AddIngredient(ingredientHolder, ingredientHolder.gameObject);
        }
    }
}
