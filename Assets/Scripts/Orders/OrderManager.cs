using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour
{
    [Header("Order Variables")]
    [SerializeField] private RecipeManager recipeManager;
    private RecipeSO[] _availableRecipes;


    private void Start()
    {
        recipeManager = FindObjectOfType<RecipeManager>();
        _availableRecipes = recipeManager.FindAvailableRecipes();
    }

    // Generate a random order for a customer
    internal RecipeSO GiveOrder(string name)
    {
        // If there are no available recipes, return
        if (_availableRecipes.Length == 0) return null;

        RecipeSO assignedOrder;

        if(name == "EvilMage")
        {
            assignedOrder = _availableRecipes[0];
        }
        else
        {
            int randomIndex = Random.Range(0, _availableRecipes.Length);
            assignedOrder = _availableRecipes[randomIndex];
        }

        return assignedOrder;
    }
} 