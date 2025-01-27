using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauldronInteraction : MonoBehaviour, IInteractable
{
    // Reference to the RecipeManager script
    private RecipeManager recipeManager;

    // Holds all the recipes that can be crafted in this Cauldron
    private RecipeSO[] craftableRecipes;

    // List of ingredients added to the cauldron
    private List<RecipeStepSO.Ingredient> addedIngredients = new List<RecipeStepSO.Ingredient>();

    // Recipe variables
    private RecipeSO curRecipe;
    private RecipeStepSO curStep;
    private RecipeStepSO nextStep;
    private RecipeStepSO.Ingredient ingredient;
    private GameObject ingredientGO;
    private int curStepIndex;
    private Renderer curPotionRend;

    [SerializeField] private Transform ingredientInsertPoint;

    //sound libraries and clips
    [Header("Sounds")]
    [SerializeField] private SFXLibrary addIngredientSounds;
    [SerializeField] private SFXLibrary FinishPotionSounds;

    [Header("Potion Throwing")]
    [SerializeField] private float throwStrength = 5f; // strength of the throw
    [SerializeField] private float throwHeight = 2f; // max height of the arc
    [SerializeField] private float throwDuration = 1f; // time to reach target

    [Header("Spoon Rotation")]
    [SerializeField] private Transform spoon;
    [Tooltip("Lower the number the slower it goes")]
    [SerializeField] private float spoonRotationSpeed = 0.6f;

    [Header("Incorrect Step Particle System")]
    [SerializeField] private ParticleSystem incorrectStepParticles;


    public void Start()
    {
        recipeManager = FindObjectOfType<RecipeManager>();
        craftableRecipes = recipeManager.FindAvailableRecipes();
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
        if (player.GetRecipeStep() == null) return;

        curStep = player.GetRecipeStep();
        ingredientGO = player.GetGameObject();


        // grabs the ingredient from the recipe step that's holding it.
        ingredient = curStep.ingredient;
        player.PutIngredientInCauldron(ingredientInsertPoint);


        if (curStepIndex == 0)
        {
            StartNewRecipe(player);
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

    #region Recipe Steps
    /// <summary>
    /// Starts a new recipe, checks if the recipe step is the first step in the list of available recipes, if its not then it handles the incorrect step
    /// </summary>
    private void StartNewRecipe(InteractionDetector player)
    {
        foreach (RecipeSO recipe in craftableRecipes)
        {
            if (recipe.steps[curStepIndex] == curStep)
            {
                curRecipe = recipe;

                if (ingredient == RecipeStepSO.Ingredient.Bottle)
                {
                    CompleteRecipe();
                    return;
                }

                curStepIndex++;
                nextStep = curRecipe.steps[curStepIndex];

                addedIngredients.Add(ingredient);
                return;
            }
        }

        HandleIncorrectStep();
    }


    /// <summary>
    ///  Advances to the next step in the recipe, checks if the ingredient is the same as the next step, if it is then it increments the current step index, if not then it handles the incorrect step
    /// </summary>
    private void AdvanceToNextStep()
    {
        if (nextStep.ingredient == ingredient)
        {
            addedIngredients.Add(ingredient);

            if (curStepIndex < curRecipe.steps.Length)
            {
                if (ingredient == RecipeStepSO.Ingredient.Bottle)
                {
                    CompleteRecipe();
                    return;
                }

                curStepIndex++;
                nextStep = curRecipe.steps[curStepIndex];
                return;
            }
        }
        else
            HandleIncorrectStep();
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

        ingredientGO.GetComponent<IngredientHolder>().enabled = false;
        ingredientGO.GetComponent<PotionOutput>().enabled = true;

        // Instantiate the completed potion prefab
        StartCoroutine(ThrowFromCauldron());
    }

    // Handles the incorrect step
    private void HandleIncorrectStep()
    {
        // Blowing up animation thing
        Debug.Log("Incorrect step");

        // Play a sound
        incorrectStepParticles.Play();

        ResetValues();
    }
    #endregion

    // Sets the fill amount & color of the potion
    void SetPotionOutput()
    {
        if (curPotionRend == null) return;
        if (curRecipe == null) return;

        // Create a new MaterialPropertyBlock and get the renderer
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        curPotionRend.GetPropertyBlock(block);

        // Set the fill amount
        block.SetFloat("_Fill", 0.6f);
        block.SetColor("_TopColor", curRecipe.potionColor);

        // Apply the property block back to the renderer
        curPotionRend.SetPropertyBlock(block);
    }

    /// <summary>
    /// Throws the potion from the cauldron, gives the thought the potion is being scooped out
    /// </summary>
    private IEnumerator ThrowFromCauldron()
    {
        yield return new WaitForSeconds(0.5f);
        SetPotionOutput();

        Vector3 startPosition = ingredientInsertPoint.position;
        Vector3 randomDireciton = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)).normalized;

        Vector3 targetPositon = startPosition + (randomDireciton * throwStrength);
        ingredientGO.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutSine);
        ingredientGO.transform.DOJump(targetPositon, throwHeight, 1, throwDuration).SetEase(Ease.OutQuad).OnComplete(() => ResetValues());
    }


    private void StirClockwise()
    {
        spoon.DORotate(new Vector3(0, -360, 0), spoonRotationSpeed, RotateMode.FastBeyond360);
        if (nextStep == null)
        {
            HandleIncorrectStep();
            return;
        }

        if (nextStep.stepType == RecipeStepSO.StepType.StirClockwise)
        {
            Debug.Log("Stirring clockwise");
            curStepIndex++;
            nextStep = curRecipe.steps[curStepIndex];
            return;
        }
        else
        {
            HandleIncorrectStep();
        }
    }

    private void StirCounterClockwise()
    {
        spoon.DORotate(new Vector3(0, 360, 0), spoonRotationSpeed, RotateMode.FastBeyond360);

        if (nextStep == null)
        {
            HandleIncorrectStep();
            return;
        }

        if (nextStep.stepType == RecipeStepSO.StepType.StirCounterClockwise)
        {
            Debug.Log("Stirring counter clockwise");
            curStepIndex++;
            nextStep = curRecipe.steps[curStepIndex];
        }
        else
        {
            HandleIncorrectStep();
        }
    }

    /// <summary>
    /// Resets curStep, curRecipe, nextStep & clears the list of added ingredients
    /// </summary>
    private void ResetValues()
    {
        addedIngredients.Clear();
        curStepIndex = 0;
        curRecipe = null;
        curStep = null;
        nextStep = null;
    }
}