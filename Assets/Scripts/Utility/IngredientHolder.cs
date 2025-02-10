using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientHolder : MonoBehaviour
{
    // Holds the ingredientBeingHeld for the prefab
    public RecipeStepSO recipeStepIngredient;
    private bool addedToCauldron = false;
    PickupObject pickup;

    private void Start()
    {
        pickup = GetComponent<PickupObject>();
    }

    //Accessor method that checks if the ingredient has been added to the cauldron
    public bool AddedToCauldron()
    {
        pickup.Drop(false);
        return addedToCauldron;
    }

    //Mutator method that marks the ingredient as added to the cauldron
    public void AddToCauldron()
    {
        addedToCauldron = true;
    }
}
