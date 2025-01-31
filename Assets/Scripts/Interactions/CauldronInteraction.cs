using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class CauldronInteraction : MonoBehaviour
{
    // Reference to the RecipeManager script
    private RecipeManager recipeManager;

    // Holds all the recipes that can be crafted in this Cauldron
    private RecipeSO[] craftableRecipes;
    private List<RecipeSO> possibleRecipes = new();

    // Particles for incorrect step
    private VisualEffect incorrectStep;

    // Recipe variables
    private RecipeSO curRecipe;
    private RecipeStepSO curStep;
    private RecipeStepSO nextStep;
    private GameObject ingredientGO;
    private Renderer curPotionRend;
    private int curStepIndex;
    private int curStirAmount = 1;
    private bool canInteract;

    private bool potionCompleted;
    private int potionIndex;
    [SerializeField] private Transform cauldronFill;
    private Vector3 cauldronStartingPosition;

    [Header("Potion Insert Spot")]
    [SerializeField] private Transform ingredientInsertPoint;

    [Header("Potion Throwing")]
    [SerializeField] private float throwStrength = 5f; // strength of the throw
    [SerializeField] private float throwHeight = 2f; // max height of the arc
    [SerializeField] private float throwDuration = 1f; // time to reach target

    [Header("Spoon Rotation")]
    [Tooltip("Lower the number the slower it goes")]
    [SerializeField] private float spoonRotationSpeed = 0.6f;
    [SerializeField] private Transform spoon;
    private bool tryStirring;

    //sound libraries and clips
    [Header("Sounds")]
    [SerializeField] private SFXLibrary addIngredientSounds;
    [SerializeField] private SFXLibrary FinishPotionSounds;
    [SerializeField] private SFXLibrary incorrectStepSounds;


    public void Start()
    {
        cauldronStartingPosition = cauldronFill.transform.position;
        incorrectStep = GetComponentInChildren<VisualEffect>();

        recipeManager = FindObjectOfType<RecipeManager>();

        craftableRecipes = recipeManager.FindAvailableRecipes();
        ResetValues();
    }

    //#region Events
    private void OnEnable()
    {
        InputManager.StirClockwiseAction += StirClockwise;
        InputManager.StirCounterClockwiseAction += StirCounterClockwise;
    }

    private void OnDisable()
    {
        InputManager.StirClockwiseAction -= StirClockwise;
        InputManager.StirCounterClockwiseAction -= StirCounterClockwise;
    }
    //#endregion

    public void AddIngredient(IngredientHolder ingredientHolder, GameObject ingredientObject)
    {
        ingredientGO = ingredientObject;

        curStep = ingredientHolder.recipeStepIngredient;

        // grabs the ingredient from the recipe step that's holding it.
        ingredientGO.transform.SetParent(null);
        ingredientGO.transform.DOJump(ingredientInsertPoint.position, 1f, 1, 0.5f).SetEase(Ease.InOutSine);
        ingredientGO.transform.DOScale(Vector3.zero, 1f).SetEase(Ease.InOutSine).OnComplete(SetInactive);

        // Play a sound here
        //AudioManager.instance.sfxManager.playSFX();

        if (curStepIndex == 0)
        {
            StartNewRecipe();
            return;
        }
        else
        {
            if (nextStep != null)
            {
                AdvanceToNextStep();
                return;
            }
            else
                HandleIncorrectStep();
        }
    }

    private void SetInactive()
    {
        if (curStep.ingredient == RecipeStepSO.Ingredient.Bottle) return;
        ingredientGO.GetComponent<Rigidbody>().isKinematic = true;
        ingredientGO.transform.position = new Vector3(-100, -100, -100);
    }

    #region Recipe Steps
    /// <summary>
    /// Starts a new recipe, checks if the recipe step is the first step in the list of available recipes, if its not then it handles the incorrect step
    /// </summary>
    private void StartNewRecipe()
    {
        possibleRecipes.Clear();

        foreach (RecipeSO recipe in craftableRecipes)
        {
            if (recipe.steps[0] == curStep)
            {
                possibleRecipes.Add(recipe);
            }
        }

        if (possibleRecipes.Count == 0)
        {
            HandleIncorrectStep();
            return;
        }
        curStepIndex++;

        if (possibleRecipes.Count == 1)
        {
            curRecipe = possibleRecipes[0];

            // Check if it's a one-step recipe (like just adding a bottle)
            if (curRecipe.steps[0].ingredient == RecipeStepSO.Ingredient.Bottle)
            {
                CompleteRecipe();
                return;
            }


            nextStep = curRecipe.steps[curStepIndex]; // Set the next step
        }
    }


    /// <summary>
    ///  Advances to the next step in the recipe, checks if the ingredient is the same as the next step, if it is then it increments the current step index, if not then it handles the incorrect step
    /// </summary>
    private void AdvanceToNextStep()
    {
        if (nextStep == null)
        {
            CheckPossibleRecipes();
        }

        if (nextStep == null)
        {
            HandleIncorrectStep();
            return;
        }

        switch (nextStep.stepType)
        {
            case RecipeStepSO.StepType.StirClockwise:
            case RecipeStepSO.StepType.StirCounterClockwise:
                if (!tryStirring)
                {
                    HandleIncorrectStep();
                    return;
                }

                if (curStirAmount >= nextStep.stirAmount)
                {
                    curStepIndex++;
                    curStirAmount = 1;
                    tryStirring = false;

                    nextStep = curRecipe.steps[curStepIndex];
                }
                else
                {
                    curStirAmount++;
                    tryStirring = false;
                }
                return;

            case RecipeStepSO.StepType.AddIngredient:
                if (nextStep.ingredient == curStep.ingredient)
                {
                    curStepIndex++;

                    if (curStepIndex < curRecipe.steps.Length)
                        nextStep = curRecipe.steps[curStepIndex];
                    else
                        CompleteRecipe();
                }
                else
                {
                    HandleIncorrectStep();
                }
                return;
        }
    }


    private void CheckPossibleRecipes()
    {
        List<RecipeSO> validRecipes = new();

        foreach (RecipeSO recipe in possibleRecipes)
        {
            RecipeStepSO expectedStep = recipe.steps[curStepIndex];

            if (expectedStep.stepType == RecipeStepSO.StepType.AddIngredient)
            {
                if (!tryStirring && expectedStep.ingredient == curStep.ingredient)
                    validRecipes.Add(recipe);
            }
            else
            {
                if (tryStirring && expectedStep.stepType == curStep.stepType)
                    validRecipes.Add(recipe);
            }
        }

        possibleRecipes = validRecipes;

        if (possibleRecipes.Count == 0)
        {
            HandleIncorrectStep();
            return;
        }

        if (possibleRecipes.Count == 1)
        {
            curRecipe = possibleRecipes[0];
        }
    }

    /// <summary>
    /// Completes the recipe, sets the fill amount of the potion and throws it from the cauldron
    /// </summary>
    private void CompleteRecipe()
    {
        // Ensure ingredientGO exists
        if (ingredientGO == null) return;

        GameObject potionInside = ingredientGO.transform.GetChild(1).gameObject;
        if (potionInside == null) return;

        curPotionRend = potionInside.GetComponent<Renderer>();
        if (curPotionRend == null) return;

        ingredientGO.GetComponent<Rigidbody>().isKinematic = false;
        ingredientGO.GetComponent<IngredientHolder>().enabled = false;
        ingredientGO.GetComponent<PotionOutput>().enabled = true;
        ingredientGO.GetComponent<PotionOutput>().potionInside = curRecipe;

        // Instantiate the completed potion prefab
        StartCoroutine(ThrowFromCauldron());

    }

    // Handles the incorrect step
    private void HandleIncorrectStep()
    {
        // Blowing up animation thing
        Debug.Log("Incorrect step");

        // Play a sound here
        //AudioManager.instance.sfxManager.playSFX()

        incorrectStep.Play();

        ResetValues();
    }
    #endregion


    /// <summary>
    /// Throws the potion from the cauldron, gives the thought the potion is being scooped out
    /// </summary>
    private IEnumerator ThrowFromCauldron()
    {
        yield return new WaitForSeconds(0.5f);
        SetPotionOutput();

        // Play a sound here
        //AudioManager.instance.sfxManager.playSFX()

        // Throw the potion from the cauldron in a random direction
        Vector3 startPosition = ingredientInsertPoint.position;
        Vector3 randomDireciton = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)).normalized;
        Vector3 targetPositon = startPosition + (randomDireciton * throwStrength);

        CountPotions();
        ingredientGO.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 1f).SetEase(Ease.InOutSine);
        ingredientGO.transform.DOJump(targetPositon, throwHeight, 1, throwDuration);

    }

    // Sets the fill amount & color of the potion
    private void SetPotionOutput()
    {
        if (curPotionRend == null) return;
        if (curRecipe == null) return;

        // Create a new MaterialPropertyBlock and get the renderer
        MaterialPropertyBlock block = new();
        curPotionRend.GetPropertyBlock(block);

        // Set the fill amount
        block.SetFloat("_Fill", 0.6f);
        block.SetColor("_Color", curRecipe.potionColor);

        // Apply the property block back to the renderer
        curPotionRend.SetPropertyBlock(block);
    }

    #region Stirring Actions
    private void StirClockwise(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            if (!canInteract) return;

            tryStirring = true;
            spoon.DORotate(new Vector3(0, -360, 16), spoonRotationSpeed, RotateMode.FastBeyond360);
            AdvanceToNextStep();
        }
    }

    private void StirCounterClockwise(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            if (!canInteract) return;

            tryStirring = true;
            spoon.DORotate(new Vector3(0, 360, 16), spoonRotationSpeed, RotateMode.FastBeyond360);
            AdvanceToNextStep();
        }
    }
    #endregion

    private void CountPotions()
    {
        // If the first step is a bottle, reset the values
        if (curRecipe != null && curRecipe.steps.Length == 1 && curRecipe.steps[0].ingredient == RecipeStepSO.Ingredient.Bottle)
        {
            ResetValues();
            return;
        }

        // Decreases the fill amount of the cauldron
        cauldronFill.DOMove(cauldronFill.position - new Vector3(0, 0.11f, 0), 0.8f).OnComplete(CheckPotionCount);
    }

    // Checks if the potion is completed or if there are more potions to be made
    private void CheckPotionCount()
    {
        if (!potionCompleted)
        {
            potionIndex++;
            potionCompleted = true;
            return;
        }
        else
        {
            potionIndex++;
            if (potionIndex == 3)
            {
                ResetValues();
            }
        }
    }

    /// <summary>
    /// Resets curStep, curRecipe, nextStep & clears the list of added ingredients
    /// </summary>
    private void ResetValues()
    {
        cauldronFill.DOMove(cauldronStartingPosition, 1f);
        potionCompleted = false;
        potionIndex = 0;
        curStepIndex = 0;
        curRecipe = null;
        curStep = null;
        nextStep = null;
    }

    // using this to check if the player is in range to stir the cauldron
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            canInteract = true;
    }

    private void OnTriggerExit(Collider other)
    {
        canInteract = false;
    }
}