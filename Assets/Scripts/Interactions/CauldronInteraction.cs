using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauldronInteraction : MonoBehaviour, IInteractable
{
    // Reference to the RecipeManager script
    private RecipeManager _recipeManager;

    // Holds all the recipes that can be crafted in this Cauldron
    private RecipeSO[] _craftableRecipes;

    // List of ingredients added to the cauldron
    private List<RecipeStepSO.Ingredient> _addedIngredients = new List<RecipeStepSO.Ingredient>();

    // Recipe variables
    private RecipeSO _currentRecipe;
    private RecipeStepSO _nextStep;
    private RecipeStepSO.Ingredient _ingredient;
    private GameObject ingredientGO;
    private int _currentStepIndex;

    [SerializeField] private Transform ingredientInsertPoint;

    //sound libraries and clips
    [Header("Sounds")]
    [SerializeField] private SFXLibrary addIngredientSounds;
    [SerializeField] private SFXLibrary FinishPotionSounds;

    [Header("Potion Throwing")]
    [SerializeField] private float throwStrength = 5f; // strength of the throw
    [SerializeField] private float throwHeight = 2f; // max height of the arc
    [SerializeField] private float throwDuration = 1f; // time to reach target


    // testing

    private bool fillAmountSet = false;

    public void Start()
    {
        _recipeManager = FindObjectOfType<RecipeManager>();
        _craftableRecipes = _recipeManager.FindAvailableRecipes();
        ResetValues();
    }

    #region Events
    private void OnEnable()
    {
        Actions.OnStirClockwise += StirClockwise;
        Actions.OnStirCounterClockwise += StirCounterClockwise;
    }

    private void OnDisable()
    {
        Actions.OnStirClockwise -= StirClockwise;
        Actions.OnStirCounterClockwise -= StirCounterClockwise;
    }
    #endregion

    public void Interact(InteractionDetector player)
    {
        if (player.GetRecipeStepIngredient() == null) return;

        RecipeStepSO _recipe = player.GetRecipeStepIngredient();
        ingredientGO = player.GetGameObject();

        _ingredient = _recipe.ingredient;
        player.PutIngredientInCauldron(ingredientInsertPoint);


        if (_ingredient == RecipeStepSO.Ingredient.Bottle)
        {
            CompleteRecipe();
        }


        //player.RemoveIngredient();

        // Check if player has anything to be able to interact with the cauldron - this is for the interaction button specifically

        // if the player does have a step then they can interact with the cauldron

        // if the current step is 0 then start a new recipe

        // if it isnt then check if the next step is null, if it is then handle the incorrect step

    }


    private void StartNewRecipe(RecipeStepSO recipeStep, InteractionDetector player)
    {
        // this should check if the recipe step is the first step in the list of available recipes

        // if it is then set the current recipe to that recipe

        // set the next step to the next step in the recipe

        // increment the current step index

        // add the ingredient to the list of added ingredients

    }

    private void AdvanceToNextStep(RecipeStepSO recipeStep, InteractionDetector player)
    {
        // this should check if the recipe step is the next step in the list of available recipes
        // if it is then set the next step to the next step in the recipe
        // increment the current step index
        // add the ingredient to the list of added ingredients
        // if the next step is null then complete the recipe or handle the incorrect step
    }

    private void HandleIncorrectStep()
    {
        // Blowing up animation thing
        Debug.Log("Incorrect step");
        ResetValues();
    }

    private void CompleteRecipe()
    {
        GameObject potionInner = ingredientGO.gameObject.transform.GetChild(1).gameObject;
        Debug.Log(potionInner.name);


        Renderer renderer = potionInner.GetComponent<Renderer>();
        


        ingredientGO.GetComponent<IngredientHolder>().enabled = false;
        ingredientGO.GetComponent<PotionOutput>().enabled = true;

        StartCoroutine(ThrowFromCauldron());
        // Instantiate the completed potion prefab

        // Get the hand position of the player




        // Get the potion output component from the completed potion
        // Set the potion inside to the current recipe
        // Add the potion to the player
        // Clear the added ingredients list

        ResetValues();
    }

    void SetFillAmount(Renderer renderer, float fillAmount)
    {
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(block);

        block.SetFloat("_FillAmount", fillAmount);
        renderer.SetPropertyBlock(block);

        fillAmountSet = true;
    }

    private IEnumerator ThrowFromCauldron()
    {
        yield return new WaitForSeconds(0.5f);

        Vector3 startPosition = ingredientInsertPoint.position;

        Vector3 randomDireciton = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)).normalized;

        Vector3 targetPositon = startPosition + (randomDireciton * throwStrength);

        ingredientGO.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutSine);
        ingredientGO.transform.DOJump(targetPositon, throwHeight, 1, throwDuration).SetEase(Ease.OutQuad);
    }

    private void StirClockwise()
    {
        Debug.Log("Stirring clockwise");
        // should check if stirring clockwise is the next step in the recipe
        // if it is then increment the current step index
        // if not then handle the incorrect step
    }

    private void StirCounterClockwise()
    {
        Debug.Log("Stirring counter clockwise");
        // should check if stirring counter clockwise is the next step in the recipe
        // if it is then increment the current step index
        // if not then handle the incorrect step
    }

    /// <summary>
    /// Resets currentStep, currentRecipe, nextStep & clears the list of added ingredients
    /// </summary>
    private void ResetValues()
    {
        _addedIngredients.Clear();
        _currentStepIndex = 0;
        _currentRecipe = null;
        _nextStep = null;
    }
}