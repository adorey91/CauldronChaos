using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This manager will need to be reworked later as I think this should only handle the recipes and telling the cauldron / recipe book which recipes they can use
/// </summary>
public class RecipeManager : MonoBehaviour
{
    [Header("Available Recipes")]
    [SerializeField] private RecipeSO[] allRecipes;
    [SerializeField] private int numberOfRecipes;

    [Header("Bad Potion Prefab")]
    [SerializeField] private GameObject badPotion;


    public RecipeSO[] FindAvailableRecipes()
    {
        RecipeSO[] availableRecipes = new RecipeSO[numberOfRecipes];

        for(int i = 0; i < numberOfRecipes; i++)
        {
            availableRecipes[i] = allRecipes[i];
        }

        return availableRecipes;
    }

    public GameObject SetBadPotion() => badPotion;

    internal RecipeSO GetRandomRecipe()
    {
        return allRecipes[Random.Range(0, numberOfRecipes)];
    }

    internal RecipeSO GetRecipeByName(string recipeName)
    {
        foreach (RecipeSO recipe in allRecipes)
        {
            if (recipe.recipeName == recipeName)
                return recipe;
        }
        return null;
    }

}
