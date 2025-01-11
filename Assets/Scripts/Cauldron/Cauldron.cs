using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cauldron : MonoBehaviour, IInteractable
{
    // Reference to the RecipeManager script
    private RecipeManager _recipeManager;

    // Holds all the recipes that can be crafted in this Cauldron
    private RecipeSO[] _craftableRecipes;

    // Holds all the ingredients that are currently in the Cauldron
    private List<IngredientSO> _addedIngredients = new List<IngredientSO>();

    [Header("Bad Potion Prefab")]
    [SerializeField] private GameObject badPotion;

    // Recipe variables
    private RecipeSO _currentRecipe;
    private RecipeStepSO _nextStep;
    private int _currentStepIndex;
    private bool _isRecipeGood;

    public void Start()
    {
        _recipeManager = FindObjectOfType<RecipeManager>();
        _craftableRecipes = _recipeManager.FindAvailableRecipes();
        badPotion = _recipeManager.SetBadPotion();
    }

    public void Interact(InteractionDetector player)
    {
        if (!player.HasIngredient() || player.HasPotion()) return;

        IngredientSO ingredient = player.GetIngredient();

            Actions.OnRemoveIngredient?.Invoke();
        if (_addedIngredients.Count == 0)
        {
            StartNewRecipe(ingredient, player);
            return;
        }
        else
        {
            // If the next step is not null, process the next step else handle the incorrect step
            if (_nextStep != null)
                ProcessNextStep(ingredient, player);
            else
                HandleIncorrectStep(ingredient, player);
        }

        
    }

    private void StartNewRecipe(IngredientSO ingredient, InteractionDetector player)
    {
        // Check if the ingredientBeingHeld is the first step in any recipe
        foreach (RecipeSO recipe in _craftableRecipes)
        {
            if (recipe.steps[0].ingredient == ingredient)
            {
                _addedIngredients.Add(ingredient);
                _currentRecipe = recipe;
                _isRecipeGood = true;

                // If the ingredientBeingHeld is the last step in the recipe, complete the recipe
                if (ingredient.ingredientName == "Bottle")
                {
                    CompleteRecipe(player);
                    return;
                }

                _currentStepIndex++;
                _nextStep = recipe.steps[_currentStepIndex];

                return;
            }
        }
        _isRecipeGood = false;
        _addedIngredients.Add(ingredient);
    }

    private void ProcessNextStep(IngredientSO ingredient, InteractionDetector player)
    {

        _isRecipeGood = true;
        Debug.Log(ingredient.ingredientName + " added");

        // Check if the ingredientBeingHeld is the next step in the recipe
        if (_nextStep.ingredient == ingredient)
        {
            _addedIngredients.Add(ingredient);

            if (_currentStepIndex < _currentRecipe.steps.Length)
            {
                // If the ingredientBeingHeld is the last step in the recipe, complete the recipe
                if (ingredient.ingredientName == "Bottle")
                {
                    CompleteRecipe(player);
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
            HandleIncorrectStep(ingredient, player);
        }
    }

    private void HandleIncorrectStep(IngredientSO ingredient, InteractionDetector player)
    {
        _isRecipeGood = false;
        _addedIngredients.Add(ingredient);

        if (ingredient.ingredientName == "Bottle")
            CompleteRecipe(player);
    }

    private void CompleteRecipe(InteractionDetector player)
    {
        GameObject completedPotion;

        if (_isRecipeGood)
        {
            Debug.Log("Good potion");
            completedPotion = Instantiate(_currentRecipe.potionPrefab, player.GetHandPosition());
        }
        else
        {
            Debug.Log("Bad potion");
            completedPotion = Instantiate(badPotion, player.GetHandPosition());
        }

        PotionOutput potionOutput = completedPotion.GetComponent<PotionOutput>();

        if (_currentRecipe != null)
            potionOutput.recipeGiven = _currentRecipe;

        potionOutput.isPotionGood = _isRecipeGood;

        player.AddPotion(completedPotion);

        _addedIngredients.Clear();
        _currentStepIndex = 0;
        _currentRecipe = null;
        _nextStep = null;
    }
}