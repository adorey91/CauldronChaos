using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    [Header("Available Recipes")]
    [SerializeField] private RecipeSO[] availableRecipes;

    [Header("Bad Potion Prefab")]
    [SerializeField] private GameObject badPotion;

    // These are the ingredients that can be added to the cauldron, but will be removed later.
    [SerializeField] private IngredientSO mushroom;
    [SerializeField] private IngredientSO bottle;

    // List to hold the ingredients added to the cauldron
    [SerializeField] private List<IngredientSO> addedIngredients;

    // Recipe variables
    private RecipeSO _currentRecipe;
    private RecipeStepSO _nextStep;
    private int _currentStepIndex;
    private bool _isRecipeGood;

    public void Start()
    {
        _currentStepIndex = 0;
        _currentRecipe = null;
        _nextStep = null;
    }

    public void Update()
    {
        // All of these are for testing only. They will be removed later.

        if (Input.GetKeyDown(KeyCode.M))
        {
            AddIngredient(mushroom);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            AddIngredient(bottle);
        }
        
    }

    private void OnEnable()
    {
        Actions.AddIngredient += AddIngredient;
    }

    private void OnDisable()
    {
        Actions.AddIngredient -= AddIngredient;
    }

    private void AddIngredient(IngredientSO ingredient)
    {
        // If the recipe is null, find a recipe that starts with this ingredient
        if (_currentRecipe == null)
        {
            _isRecipeGood = false;

            foreach (RecipeSO recipe in availableRecipes)
            {
                // Check if the first step is adding this ingredient
                if (recipe.steps[0].stepType == RecipeStepSO.StepType.AddIngredient &&
                    recipe.steps[0].ingredient == ingredient)
                {
                    addedIngredients.Add(ingredient);
                    _currentRecipe = recipe;
                    _isRecipeGood = true;

                    if (recipe.recipeName == "Potion of Hydration")
                    {
                        RecipeOutput(); // Immediately create the potion
                        return;         // Exit early
                    }

                    _currentStepIndex++;
                    _nextStep = _currentRecipe.steps[_currentStepIndex];
                    Debug.Log($"Recipe started: {_currentRecipe.name}");
                    return;
                }
            }
        }
        // if the recipe is not null, check if the ingredient matches the next step
        else
        {
            if (_nextStep.ingredient == ingredient && _isRecipeGood)
            {
                addedIngredients.Add(ingredient);

                if (ingredient.ingredientName == "Bottle")
                {
                    RecipeOutput();
                    return;
                }

                if (_currentStepIndex > _currentRecipe.steps.Length)
                {
                    _currentStepIndex++;
                }

                _nextStep = _currentRecipe.steps[_currentStepIndex];
                Debug.Log($"Ingredient added: {ingredient.ingredientName}");
                _isRecipeGood = true;
            }
            else
            {
                Debug.Log("This ingredient doesn't match the recipe.");
                addedIngredients.Add(ingredient);

                _isRecipeGood = false;
            }
        }

    }


    public void RecipeOutput()
    {
        if (_isRecipeGood)
        {
            Debug.Log("Good potion");
            Instantiate(_currentRecipe.potionPrefab, Vector3.up, Quaternion.identity);
        }
        else
        {
            Debug.Log("Bad potion");
            GameObject potion = Instantiate(badPotion, Vector3.up, Quaternion.identity);
            BadPotion badPotionScript = potion.GetComponent<BadPotion>();
            badPotionScript.recipeFailed = _currentRecipe;
        }

        addedIngredients.Clear();
        _currentStepIndex = 0;
        _currentRecipe = null;
        _nextStep = null;
    }

    public RecipeSO[] FindAvailableRecipes()
    {
        return availableRecipes;
    }
}
