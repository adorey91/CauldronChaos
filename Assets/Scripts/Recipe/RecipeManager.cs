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

    [SerializeField] private GameObject recipeBook;

    private void OnEnable()
    {
        Actions.OnToggleRecipeBook += ToggleRecipeBook;   
    }

    private void OnDisable()
    {
        Actions.OnToggleRecipeBook -= ToggleRecipeBook;
    }

    public RecipeSO[] FindAvailableRecipes()
    {
        RecipeSO[] availableRecipes = new RecipeSO[numberOfRecipes];

        for(int i = 0; i < numberOfRecipes; i++)
        {
            availableRecipes[i] = allRecipes[i];
        }

        return availableRecipes;
    }

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

    private void ToggleRecipeBook()
    {
        if (recipeBook.activeSelf)
        {
            recipeBook.SetActive(false);
            InputManager.instance.MoveInputAction.Enable();
            InputManager.instance.NextPageInputAction.Disable();
            InputManager.instance.PreviousPageInputAction.Disable();
        }
        else
        {
            recipeBook.SetActive(true);
            //InputManager.instance.MoveInputAction.Disable();
            InputManager.instance.NextPageInputAction.Enable();
            InputManager.instance.PreviousPageInputAction.Enable();
        }
    }
}
