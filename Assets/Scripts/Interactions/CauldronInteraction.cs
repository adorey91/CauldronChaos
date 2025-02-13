using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
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
    private string nextStep;
    private string currentStep;
    private List<string> possibleNextSteps = new();

    // Particles for incorrect step
    private VisualEffect incorrectStep;

    // Recipe variables
    private RecipeSO recipe;
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

    //sound libraries and clips
    [Header("Sounds")]
    [SerializeField] private SFXLibrary addIngredientSounds;
    [SerializeField] private SFXLibrary FinishPotionSounds;
    [SerializeField] private SFXLibrary incorrectStepSounds;
    [SerializeField] private SFXLibrary stirSouds;



    public void Start()
    {
        cauldronStartingPosition = cauldronFill.transform.localPosition;
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

    private void OnDestroy()
    {
        DOTween.KillAll();
    }
    #endregion

    #region Recipe Steps
    public void AddIngredient(IngredientHolder ingredientHolder, GameObject ingredientObject)
    {
        ingredientGO = ingredientObject;

        currentStep = null;

        currentStep = ingredientHolder.recipeStepIngredient.stepName;

        // grabs the ingredient from the _recipes step that's holding it.
        ingredientGO.transform.DOJump(ingredientInsertPoint.position, 1f, 1, 0.5f).SetEase(Ease.InOutSine);
        ingredientGO.transform.DOScale(Vector3.zero, 1f).SetEase(Ease.InOutSine).OnComplete(SetInactive);

        // Play a sound here
        AudioManager.instance.sfxManager.PlaySFX(SFX_Type.StationSounds, addIngredientSounds.PickAudioClip(), true);

        if (curStepIndex == 0)
        {
            StartNewRecipe();
        }
        else
        {
            AdvanceToNextStep();
        }

    }

    private void Stir()
    {
        //play stirring sound
        AudioManager.instance.sfxManager.PlaySFX(SFX_Type.StationSounds, stirSouds.PickAudioClip(), false);

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
        // Play a sound here
        //AudioManager.instance.sfxManager.playSFX()
        ingredientGO.GetComponent<Rigidbody>().isKinematic = true;

        incorrectStep.Play();
        ResetValues();
    }

    private void FindPossibleRecipes()
    {
        List<RecipeSO> filterRecipes = new();
        possibleNextSteps.Clear();

        foreach (RecipeSO _recipes in possibleRecipes)
        {
            if (curStepIndex >= _recipes.steps.Length)
                continue;

            if (_recipes.steps[curStepIndex].stepName == currentStep)
            {
                filterRecipes.Add(_recipes);
                curStirAmount = 0;

                // Ensure next step exists
                if (curStepIndex + 1 < _recipes.steps.Length)
                {
                    nextStep = _recipes.steps[curStepIndex + 1].stepName;
                    possibleNextSteps.Add(nextStep);
                }
                else
                {
                    nextStep = null;
                }

                // Check if it's the final step
                if (_recipes.steps[curStepIndex].stepName == "Bottle_Potion")
                {
                    recipe = _recipes;
                    CompletePotion();
                    return;
                }
            }
        }

        possibleRecipes = filterRecipes;

        if (possibleRecipes.Count == 0)
        {
            HandleIncorrectStep();
            return;
        }

        curStepIndex++;

        if (possibleRecipes.Count == 1)
        {
            recipe = possibleRecipes[0];

            if (curStepIndex < recipe.steps.Length)
            {
                nextStep = recipe.steps[curStepIndex].stepName;
            }
        }

        //Debug.Log($"Updated nextStep: {nextStep}");
    }


    private void StartNewRecipe()
    {
        possibleRecipes.Clear();
        possibleNextSteps.Clear();

        foreach (RecipeSO recipe in craftableRecipes)
        {
            if (recipe.steps.Length > 0 && recipe.steps[0].stepName == currentStep)
            {
                possibleRecipes.Add(recipe);
                if (recipe.steps.Length > 1)
                    possibleNextSteps.Add(recipe.steps[1].stepName);
            }
        }

        if (possibleRecipes.Count == 0)
        {
            HandleIncorrectStep();
            return;
        }

        curStepIndex = 1; // Move to next step

        if (possibleRecipes.Count == 1)
        {
            recipe = possibleRecipes[0];

            if (recipe.steps.Length > 1)
            {
                nextStep = recipe.steps[curStepIndex].stepName;
            }

            if (recipe.steps[0].stepName == "Bottle_Potion")
            {
                CompletePotion();
            }
        }
    }


    private void AdvanceToNextStep()
    {
        if (recipe == null)
        {
            FindPossibleRecipes();
            return;
        }

        if (curStepIndex >= recipe.steps.Length)
        {
            HandleIncorrectStep();
            return;
        }

        //Debug.Log($"Current Step: {currentStep}, Expected Step: {recipe.steps[curStepIndex].stepName}");

        // If an unexpected ingredient is added, trigger incorrect step handling
        if (recipe.steps[curStepIndex].stepName != currentStep)
        {
            //Debug.LogError($"Incorrect step detected! Current: {currentStep}, Expected: {recipe.steps[curStepIndex].stepName}");
            HandleIncorrectStep();
            return;
        }

        if (recipe.steps[curStepIndex].stepName == "Bottle_Potion")
        {
            CompletePotion();
            return;
        }

        curStirAmount = 0;
        curStepIndex++;

        if (curStepIndex < recipe.steps.Length)
        {
            nextStep = recipe.steps[curStepIndex].stepName;
            //Debug.Log($"Next expected step: {nextStep}");
        }
    }




    #endregion

    #region Potion Completion
    private void CompletePotion()
    {

        if (ingredientGO == null)
            return;

        if (ingredientGO.transform.childCount < 2)
            return;

        GameObject potionInside = ingredientGO.transform.GetChild(1).gameObject;
        if (potionInside == null)
            return;

        curPotionRend = potionInside.GetComponent<Renderer>();
        if (curPotionRend == null)
            return;

        ingredientGO.GetComponent<Rigidbody>().isKinematic = false;
        Destroy(ingredientGO.GetComponent<IngredientHolder>());
        ingredientGO.GetComponent<PotionOutput>().enabled = true;
        ingredientGO.GetComponent<PotionOutput>().potionInside = recipe;
        ingredientGO.transform.SetParent(null);

        // Instantiate the completed potion prefab
        StartCoroutine(ThrowPotion());
    }

    private IEnumerator ThrowPotion()
    {
        yield return new WaitForSeconds(0.3f);
        ingredientGO.GetComponent<PotionOutput>().SetPotionColor();

        // Play a sound here
        //AudioManager.instance.sfxManager.PlaySFX(SFX_Type.StationSounds, FinishPotionSounds.PickAudioClip(), true);

        // throw the potion from the cauldron in a random direction
        Vector3 startPosition = ingredientInsertPoint.position;
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 1, Random.Range(-1f, 1f)).normalized;
        Vector3 targetPosition = startPosition + randomDirection * throwStrength;

        if(recipe.recipeName != "Potion of Hydration")
            CountPotions();
        else
            ResetValues();

        ingredientGO.transform.DOScale(new Vector3(1f, 1f, 1f), 1f).SetEase(Ease.InOutSine);
        ingredientGO.transform.DOJump(targetPosition, throwHeight, 1, throwDuration);

    }


    private void CountPotions()
    {
        if (recipe != null && recipe.steps[0].ingredient == RecipeStepSO.Ingredient.Bottle)
        {
            if (potionIndex < 3) // Ensure we don't reset too soon
            {
                potionIndex++;
            }
            else
            {
                ResetValues();
            }
        }

        cauldronFill.DOLocalMove(cauldronFill.localPosition - new Vector3(0, 0.11f, 0), 0.8f).OnComplete(CheckPotionCount);
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

            // Ensure we only reset if all 3 potions were pulled
            if (potionIndex == 3)
                ResetValues();
        }
    }


    #endregion

    private void SetInactive()
    {
        if (currentStep == "Bottle_Potion") return;
        ingredientGO.GetComponent<Rigidbody>().isKinematic = true;
    }

    internal void GoblinInteraction()
    {
        cauldronFill.DOLocalMove(cauldronFill.localPosition - new Vector3(0, 0.11f * 2, 0), 1f).SetEase(Ease.InOutSine).OnComplete(ResetValues);
    }


    internal void ResetValues()
    {
        cauldronFill.DOLocalMove(cauldronStartingPosition, 1f);
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

            curStirAmount++;

            currentStep = "Stir_C_1";  
            //Debug.Log($"Stirring Clockwise: {currentStep}");

            spoon.DORotate(new Vector3(0, 360, 16), spoonRotationSpeed, RotateMode.FastBeyond360);

            Stir();
        }
    }

    private void StirCounterClockwise(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            if (!canInteract) return;

            curStirAmount++;

            currentStep = "Stir_CC_1"; 
            //Debug.Log($"Stirring CounterClockwise: {currentStep}");

            spoon.DORotate(new Vector3(0, -360, 16), spoonRotationSpeed, RotateMode.FastBeyond360);
            Stir();
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