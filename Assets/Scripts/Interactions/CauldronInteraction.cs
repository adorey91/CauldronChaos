using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using static RecipeStepSO;

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
    private RecipeSO recipe;
    private RecipeStepSO curStep;
    private RecipeStepSO nextStep;
    private GameObject ingredientGO;
    private Renderer curPotionRend;
    private int curStepIndex = 0;
    private int curStirAmount = 0;
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
    [SerializeField] private RecipeStepSO stirCC;
    [SerializeField] private RecipeStepSO stirC;
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
        //ResetValues();
    }

    #region Events
    private void OnEnable()
    {
        InputManager.StirClockwiseAction += StirClockwise;
        InputManager.StirCounterClockwiseAction += StirCounterClockwise;
        //Actions.OnResetValues += ResetValues;
    }

    private void OnDisable()
    {
        InputManager.StirClockwiseAction -= StirClockwise;
        InputManager.StirCounterClockwiseAction -= StirCounterClockwise;
        //Actions.OnResetValues -= ResetValues;
    }
    #endregion

    #region Recipe Steps
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
        }
        else
        {
            AdvanceToNextStep();
        }

    }

    private void Stir(bool isClockwise)
    {
        if (recipe == null)
        {
            FindPossibleRecipes();
            return;
        }

        AdvanceToNextStep();

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

    private void FindPossibleRecipes()
    {
        List<RecipeSO> filterRecipes = new();

        foreach (RecipeSO recipe in possibleRecipes)
        {
            if (recipe.steps[curStepIndex].action == ActionType.AddIngredient)
            {
                if (recipe.steps[curStepIndex].ingredient == curStep.ingredient)
                {
                    filterRecipes.Add(recipe);
                }
            }
            else if (recipe.steps[curStepIndex].action == ActionType.Stir)
            {
                if (recipe.steps[curStepIndex].stirAmount <= curStirAmount && recipe.steps[curStepIndex].isClockwise == curStep.isClockwise)
                {
                    filterRecipes.Add(recipe);
                }
            }
        }

        Debug.Log("Possible recipes: " + filterRecipes.Count);
        possibleRecipes.Clear();
        possibleRecipes = filterRecipes;

        if (possibleRecipes.Count == 0)
        {
            HandleIncorrectStep();
            return;
        }

        if (possibleRecipes.Count == 1)
        {
            recipe = possibleRecipes[0];

            if (recipe.steps[curStepIndex].ingredient == Ingredient.Bottle)
            {
                CompletePotion();
                return;
            }

            nextStep = recipe.steps[curStepIndex + 1];
        }
    }

    private void StartNewRecipe()
    {
        possibleRecipes.Clear();

        foreach (RecipeSO recipe in craftableRecipes)
        {
            if (recipe.steps[0].ingredient == curStep.ingredient)
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
            recipe = possibleRecipes[0];
            // Check if it's a one-step recipe (like just adding a bottle)
            if (recipe.steps[0].ingredient == RecipeStepSO.Ingredient.Bottle)
            {
                CompletePotion();
                return;
            }

            nextStep = recipe.steps[curStepIndex];
        }

    }

    private void AdvanceToNextStep()
    {
        if (recipe == null)
        {
            FindPossibleRecipes();
            return;
        }

        switch (curStep.action)
        {
            case ActionType.AddIngredient:
                if (nextStep.ingredient == curStep.ingredient)
                {
                    curStepIndex++;

                    if (curStep.ingredient == Ingredient.Bottle)
                    {
                        CompletePotion();
                        return;
                    }
                }
                else
                {
                    HandleIncorrectStep();
                    return;
                }
                break;
            case ActionType.Stir:
                if (nextStep.isClockwise == curStep.isClockwise)
                {
                    if (nextStep.stirAmount > curStep.stirAmount)
                    {
                        curStirAmount++;
                    }
                    if (nextStep.stirAmount == curStep.stirAmount)
                    {
                        curStepIndex++;
                        curStirAmount = 0;
                    }
                }
                else
                {
                    HandleIncorrectStep();
                }

                break;
        }
    }

    #endregion

    #region Potion Completion
    private void CompletePotion()
    {
        if (ingredientGO == null) return;

        GameObject potionInside = ingredientGO.transform.GetChild(1).gameObject;
        if (potionInside == null) return;

        curPotionRend = potionInside.GetComponent<Renderer>();
        if (curPotionRend == null) return;

        ingredientGO.GetComponent<Rigidbody>().isKinematic = false;
        ingredientGO.GetComponent<IngredientHolder>().enabled = false;
        ingredientGO.GetComponent<PotionOutput>().enabled = true;
        ingredientGO.GetComponent<PotionOutput>().potionInside = recipe;

        // Instantiate the completed potion prefab
        StartCoroutine(ThrowPotion());

    }

    private IEnumerator ThrowPotion()
    {
        yield return new WaitForSeconds(0.3f);
        SetPotionOutput();

        // Play a sound here
        //AudioManager.instance.sfxManager.playSFX();

        // throw the potion from the cauldron in a random direction
        Vector3 startPosition = ingredientInsertPoint.position;
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 1, Random.Range(-1f, 1f)).normalized;
        Vector3 targetPosition = startPosition + randomDirection * throwStrength;

        CountPotions();

        ingredientGO.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 1f).SetEase(Ease.InOutSine);
        ingredientGO.transform.DOJump(targetPosition, throwHeight, 1, throwDuration);

    }

    private void SetPotionOutput()
    {
        if (curPotionRend == null) return;
        if (recipe == null) return;

        // Create a new MaterialPropertyBlock and get the renderer
        MaterialPropertyBlock block = new();
        curPotionRend.GetPropertyBlock(block);

        // Set the fill amount
        block.SetFloat("_Fill", 0.6f);
        block.SetColor("_Color", recipe.potionColor);

        // Apply the property block back to the renderer
        curPotionRend.SetPropertyBlock(block);
    }

    private void CountPotions()
    {
        if (recipe != null && recipe.steps[0].ingredient == RecipeStepSO.Ingredient.Bottle)
        {
            ResetValues();
        }

        cauldronFill.DOMove(cauldronFill.position - new Vector3(0, 0.11f, 0), 0.8f).OnComplete(CheckPotionCount);
    }

    private void CheckPotionCount()
    {
        if (!potionCompleted)
        {
            potionIndex++;
            potionCompleted = true;
        }
        else
        {
            potionIndex++;

            if (potionIndex == 3)
                ResetValues();
        }
    }

    #endregion

    private void SetInactive()
    {
        if (curStep.ingredient == RecipeStepSO.Ingredient.Bottle) return;
        ingredientGO.GetComponent<Rigidbody>().isKinematic = true;
        ingredientGO.transform.position = new Vector3(-100, -100, -100);
    }

    private void ResetValues()
    {
        cauldronFill.DOMove(cauldronStartingPosition, 1f);
        potionCompleted = false;
        curStirAmount = 0;
        potionIndex = 0;
        curStepIndex = 0;
        recipe = null;
        nextStep = null;
    }

    #region Stirring
    private void StirClockwise(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            if (!canInteract) return;
            curStep = stirC;

            curStirAmount++;

            spoon.DORotate(new Vector3(0, -360, 16), spoonRotationSpeed, RotateMode.FastBeyond360);
            Stir(true);
        }
    }

    private void StirCounterClockwise(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            if (!canInteract) return;

            curStirAmount++;

            curStep = stirCC;
            spoon.DORotate(new Vector3(0, 360, 16), spoonRotationSpeed, RotateMode.FastBeyond360);
            Stir(false);
        }
    }

    #endregion

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