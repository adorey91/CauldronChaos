using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This manager will need to be reworked later as I think this should only handle the recipes and telling the cauldron / recipe book which recipes they can use
/// </summary>
public class RecipeManager : MonoBehaviour
{
    [Header("Available Recipes")]
    [SerializeField] private RecipeSO[] allRecipes;
    [SerializeField] private int numberOfRecipes;

    [SerializeField] private GameObject recipeBookUi;

    [SerializeField] private bool useAllRecipes;
    [SerializeField] private GameObject closeButton;

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

        if (useAllRecipes)
        {
            availableRecipes = allRecipes;
        }
        else
        {
            for(int i = 0; i < numberOfRecipes; i++)
            {
                availableRecipes[i] = allRecipes[i];
            }
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

    public void ToggleRecipeBook()
    {

        if (recipeBookUi.activeSelf)
        {
           Cursor.lockState = CursorLockMode.Locked;

            recipeBookUi.SetActive(false);
            InputManager.instance.MoveInputAction.Enable();
            InputManager.instance.NextPageInputAction.Disable();
            InputManager.instance.PreviousPageInputAction.Disable();
        }
        else
        {
            if (Gamepad.current != null)
                Cursor.lockState = CursorLockMode.Locked;
            else
                Cursor.lockState = CursorLockMode.Confined;

            recipeBookUi.SetActive(true);
            ControllerSelect.SelectRecipeButton(closeButton);
            InputManager.instance.MoveInputAction.Disable();
            InputManager.instance.NextPageInputAction.Enable();
            InputManager.instance.PreviousPageInputAction.Enable();
        }
    }
}
