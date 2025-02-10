using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour
{
    [Header("Order Variables")]
    [SerializeField] private RecipeManager recipeManager;
    private List<RecipeSO> availableRecipes = new();


    private void Start()
    {
        recipeManager = FindObjectOfType<RecipeManager>();
        GetAvailableRecipes();
    }

    private void GetAvailableRecipes()
    {
        foreach (RecipeSO recipe in recipeManager.FindAvailableRecipes())
        {
            for(int i = 0; i < recipe.weight; i++)
            {
                availableRecipes.Add(recipe);
            }
        }
    }

    // Generate a random order for a customer
    internal RecipeSO GiveOrder(string name)
    {
        // If there are no available recipes, return
        if (availableRecipes.Count == 0) return null;

        RecipeSO assignedOrder;

        if(name == "Evil Mage")
        {
            assignedOrder = availableRecipes[0];
        }
        else
        {
            int randomIndex = Random.Range(0, availableRecipes.Count);
            assignedOrder = availableRecipes[randomIndex];
        }

        return assignedOrder;
    }
} 