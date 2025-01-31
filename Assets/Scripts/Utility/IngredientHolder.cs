using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientHolder : MonoBehaviour
{
    // Holds the ingredientBeingHeld for the prefab
    public RecipeStepSO recipeStepIngredient;
    private bool addedToCauldron = false; 

    //Accessor method that checks if the ingredient has been added to the cauldron
    public bool AddedToCauldron()
    {
        return addedToCauldron;
    }

    //Mutator method that marks the ingredient as added to the cauldron
    public void AddToCauldron()
    {
        addedToCauldron = true;
    }
}
