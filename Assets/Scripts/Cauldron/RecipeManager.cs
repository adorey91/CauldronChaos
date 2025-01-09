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
    [Header("For Testing Only")]
    [SerializeField] private IngredientSO mushroom;
    [SerializeField] private IngredientSO bottle;
    [SerializeField] private IngredientSO unknown;

    // List to hold the ingredients added to the cauldron
    private List<IngredientSO> addedIngredients = new();

    // Recipe variables
    private RecipeSO _currentRecipe;
    private RecipeStepSO _nextStep;
    private int _currentStepIndex;
    private bool _isRecipeGood;

    public void Start()
    {
        addedIngredients.Clear();
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
        if (Input.GetKeyDown(KeyCode.U))
        {
            AddIngredient(unknown);
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
        // If the list is empty, check if the ingredient is the first step in any recipe
        if (addedIngredients.Count == 0)
        {
            StartNewRecipe(ingredient);
            return;
        }
        else
        {
            // If the next step is not null, process the next step else handle the incorrect step
            if (_nextStep != null)
                ProcessNextStep(ingredient);
            else
                HandleIncorrectStep(ingredient);
        }
    }

    private void StartNewRecipe(IngredientSO ingredient)
    {
        // Check if the ingredient is the first step in any recipe
        foreach (RecipeSO recipe in availableRecipes)
        {
            if (recipe.steps[0].ingredient == ingredient)
            {
                addedIngredients.Add(ingredient);
                _currentRecipe = recipe;
                _isRecipeGood = true;

                // If the ingredient is the last step in the recipe, complete the recipe
                if (ingredient.ingredientName == "Bottle")
                {
                    CompleteRecipe();
                    return;
                }

                _currentStepIndex++;
                _nextStep = recipe.steps[_currentStepIndex];

                return;
            }
        }
        _isRecipeGood = false;
        addedIngredients.Add(ingredient);
    }

    private void ProcessNextStep(IngredientSO ingredient)
    {
        _isRecipeGood = true;
        Debug.Log(ingredient.ingredientName + " added");

        // Check if the ingredient is the next step in the recipe
        if (_nextStep.ingredient == ingredient)
        {
            addedIngredients.Add(ingredient);

            if (_currentStepIndex < _currentRecipe.steps.Length)
            {
                // If the ingredient is the last step in the recipe, complete the recipe
                if (ingredient.ingredientName == "Bottle")
                {
                    CompleteRecipe();
                    return;
                }
                
                _currentStepIndex++;
                _nextStep = _currentRecipe.steps[_currentStepIndex];
                Debug.Log("Next step: " + _currentStepIndex);
            }
            
        }
        else
        {
            _nextStep = null;
            HandleIncorrectStep(ingredient);
        }
    }

    private void HandleIncorrectStep(IngredientSO ingredient)
    {
        _isRecipeGood = false;
        addedIngredients.Add(ingredient);

        if (ingredient.ingredientName == "Bottle")
            CompleteRecipe();
    }

    public void CompleteRecipe()
    {
        GameObject completedPotion;

        if (_isRecipeGood)
        {
            Debug.Log("Good potion");
            completedPotion = Instantiate(_currentRecipe.potionPrefab, Vector3.up, Quaternion.identity);
        }
        else
        {
            Debug.Log("Bad potion");
            completedPotion = Instantiate(badPotion, Vector3.up, Quaternion.identity);
        }

        PotionOutput potionOutput = completedPotion.GetComponent<PotionOutput>();
        
        if(_currentRecipe != null)
            potionOutput.recipeGiven = _currentRecipe;

        potionOutput.isPotionGood = _isRecipeGood;

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
