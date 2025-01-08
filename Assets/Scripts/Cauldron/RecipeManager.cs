using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    [SerializeField] private RecipeSO[] availableRecipes;
    private List<IngredientSO> addedIngredients;
    private bool _isRecipeGood;
  

    private void AddIngredient(IngredientSO ingredient)
    {
        foreach (RecipeSO recipe in availableRecipes)
        {
            if(ingredient.ingredientName != "Bottle")
            {
                if (recipe.ingredient[0] == ingredient)
                    _isRecipeGood = true;
                else
                    _isRecipeGood = false;
            }
            else
            {
                if (addedIngredients == 0)
                    _isRecipeGood = false;

                RecipeOutput();
            }
        }
    }

    public void RecipeOutput()
    {
        if(_isRecipeGood)
        {
            Debug.Log("Good potion");
        }
        else
        {
            Debug.Log("Bad potion");
        }
    }
}
