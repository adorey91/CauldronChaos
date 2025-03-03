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
    // List of possible recipes
    private List<RecipeSO> possibleRecipes = new();
    private string nextStep;
    private string currentStep;
    // List of possible next steps
    private List<string> possibleNextSteps = new();

    // Particles for incorrect step
    private VisualEffect incorrectStep;

    // Recipe variables
    private RecipeSO recipe;
    private GameObject ingredientGO;
    private int curStepIndex = 0;
    private bool canInteract;
    private bool potionCompleted;
    private int potionIndex;

    [Header("Cauldron Fill")]
    [SerializeField] private Transform cauldronFill;
    private Color cauldronFillDefaultColor;
    private Material cauldronFillMaterial;
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
        // Set the starting position of the cauldron
        cauldronStartingPosition = cauldronFill.transform.localPosition;

        cauldronFillMaterial = cauldronFill.GetComponent<MeshRenderer>().material;
        cauldronFillDefaultColor = cauldronFillMaterial.color;

        // Get the incorrect step particles
        incorrectStep = GetComponentInChildren<VisualEffect>();

        // Get the RecipeManager script & set the craftable recipes
        recipeManager = FindObjectOfType<RecipeManager>();
        craftableRecipes = recipeManager.FindAvailableRecipes();
    }

    #region OnEnable / OnDisable / OnDestroy Events
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

    private void OnDestroy()
    {
        InputManager.StirClockwiseAction -= StirClockwise;
        InputManager.StirCounterClockwiseAction -= StirCounterClockwise;
        DOTween.KillAll();
    }
    #endregion

    #region Recipe Steps
    public void AddIngredient(PickupObject ingredientHolder, GameObject ingredientObject)
    {
        // Set the ingredient to the current ingredient
        ingredientGO = ingredientObject;

        // Set the current step to the ingredient's step
        currentStep = null;
        currentStep = ingredientHolder.recipeIngredient.stepName;

        // grabs the ingredient from the _recipes step that's holding it.
        Sequence ingredientSequence = DOTween.Sequence();

        if (ingredientGO != null)
        {
            transform.DOScale(1.2f, 0.08f).SetLoops(2, LoopType.Yoyo);
            ingredientSequence.Append(ingredientGO.transform.DOLocalJump(ingredientInsertPoint.position, 1f, 1, 0.5f).SetEase(Ease.InOutSine))
                         .Join(ingredientGO.transform.DOScale(Vector3.zero, 1f).SetEase(Ease.InOutSine))
                         .OnComplete(SetInactive); // Call SetInactive after both tweens finish
        }

        // Play a sound here
        AudioManager.instance.sfxManager.PlaySFX(SFX_Type.StationSounds, addIngredientSounds.PickAudioClip(), true);

        // Check if it's the first step in the recipe or not
        if (curStepIndex == 0)
            StartNewRecipe();
        else
            AdvanceToNextStep();
    }

    private void Stir()
    {
        //play stirring sound
        AudioManager.instance.sfxManager.PlaySFX(SFX_Type.StationSounds, stirSouds.PickAudioClip(), false);

        // If the recipe is null, find the possible recipes
        if (recipe == null)
        {
            FindPossibleRecipes();
            return;
        }

        // If the recipe is not null, advance to the next step
        AdvanceToNextStep();
    }

    // Handles the incorrect step
    private void HandleIncorrectStep()
    {
        // Play a sound here
        //AudioManager.Instance.sfxManager.playSFX()
        if (ingredientGO != null)
            ingredientGO.GetComponent<Rigidbody>().isKinematic = true;

        incorrectStep.Play();
        ResetValues();
    }

    // Find the possible recipes based on the current step
    private void FindPossibleRecipes()
    {
        List<RecipeSO> filterRecipes = new();
        possibleNextSteps.Clear();

        // Loop through all possible recipes
        foreach (RecipeSO _recipes in possibleRecipes)
        {
            // Ensure the current step index is within the bounds of the recipe steps
            if (curStepIndex >= _recipes.steps.Length)
                continue;

            // Check if the current step matches the recipe step
            if (_recipes.steps[curStepIndex].stepName == currentStep)
            {
                filterRecipes.Add(_recipes);

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

        // if there's only one possible recipe, set it as the recipe
        if (possibleRecipes.Count == 1)
        {
            recipe = possibleRecipes[0];

            if (curStepIndex < recipe.steps.Length)
            {
                nextStep = recipe.steps[curStepIndex].stepName;
            }
        }
    }

    // Start a new recipe based on the current step
    private void StartNewRecipe()
    {
        possibleRecipes.Clear();
        possibleNextSteps.Clear();

        //cauldronFillMaterial.color = ingredientGO.GetComponent<PickupObject>().ingredientColor;
        cauldronFillMaterial.color = Color.Lerp(cauldronFillMaterial.color, ingredientGO.GetComponent<PickupObject>().ingredientColor, 3f);

        // Loop through all possible recipes
        foreach (RecipeSO recipe in craftableRecipes)
        {
            if (recipe.steps.Length > 0 && recipe.steps[0].stepName == currentStep)
            {
                possibleRecipes.Add(recipe);
                if (recipe.steps.Length > 1)
                    possibleNextSteps.Add(recipe.steps[1].stepName);
            }
        }

        // If no possible recipes were found, handle the incorrect step
        if (possibleRecipes.Count == 0)
        {
            HandleIncorrectStep();
            return;
        }

        curStepIndex = 1; // Move to next step

        // if there's only one possible recipe, set it as the recipe
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

    // Advance to the next step in the recipe
    private void AdvanceToNextStep()
    {
        // If the recipe is null, find the possible recipes
        if (recipe == null)
        {
            FindPossibleRecipes();
            return;
        }

        // Ensure the current step index is within the bounds of the recipe steps
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

        // Check if it's the final step
        if (recipe.steps[curStepIndex].stepName == "Bottle_Potion")
        {
            CompletePotion();
            return;
        }

        curStepIndex++;

        // Set the next step
        if (curStepIndex < recipe.steps.Length)
        {
            nextStep = recipe.steps[curStepIndex].stepName;
            //Debug.Log($"Next expected step: {nextStep}");
        }
    }
    #endregion

    #region Potion Completion
    // Complete the potion and throw it
    private void CompletePotion()
    {
        if (ingredientGO == null) return;

        ingredientGO.GetComponent<Rigidbody>().isKinematic = false;
        ingredientGO.GetComponent<PotionOutput>().enabled = true;
        ingredientGO.GetComponent<PotionOutput>().potionInside = recipe;
        ingredientGO.transform.SetParent(null);

        // Instantiate the completed potion prefab
        StartCoroutine(ThrowPotion());
    }

    private IEnumerator ThrowPotion()
    {
        GameObject thrownPotion;
        thrownPotion = ingredientGO;
        ingredientGO = null;

        yield return new WaitForSeconds(0.3f);
        
        
        if(thrownPotion.TryGetComponent<PotionOutput>(out var potionOutput))
            potionOutput.SetPotionColor();
        else
            Debug.LogError("PotionOutput script not found on the potion");

        // Play a sound here
        AudioManager.instance.sfxManager.PlaySFX(SFX_Type.StationSounds, FinishPotionSounds.PickAudioClip(), true);

        // throw the potion from the cauldron in a random direction
        Vector3 startPosition = ingredientInsertPoint.position;
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 1, Random.Range(-1f, 1f)).normalized;
        Vector3 targetPosition = startPosition + randomDirection * throwStrength;

        if (recipe.recipeName != "Potion of Hydration")
            CountPotions();
        else
            ResetValues();

        if(thrownPotion != null)
        {
            Sequence throwSequence = DOTween.Sequence();

            // Scale and throw at the same time
            throwSequence.Append(thrownPotion.transform.DOScale(new Vector3(1f, 1f, 1f), 1f).SetEase(Ease.InOutSine))
                         .Join(thrownPotion.transform.DOLocalJump(targetPosition, throwHeight, 1, throwDuration));
        }
    }

    // Count the potions and reset the values
    private void CountPotions()
    {
        //Debug.Log("Potion Counted " + potionIndex);
        if (potionIndex < 2) // Ensure we don't reset too soon
        {
            potionIndex++;
            cauldronFill.DOLocalMove(cauldronFill.localPosition - new Vector3(0, 0.11f, 0), 0.8f);
        }
        else
            ResetValues();
    }
    #endregion

    private void SetInactive()
    {
        if (currentStep == "Bottle_Potion") return;

        if (ingredientGO != null)
        {
            DOTween.Kill(ingredientGO.transform);
            Destroy(ingredientGO);
        }
    }

    internal void GoblinInteraction()
    {
        recipe = null;
        cauldronFill.DOLocalMove(cauldronFill.localPosition - new Vector3(0, 0.11f * 2, 0), 1f).SetEase(Ease.InOutSine).OnComplete(ResetValues);
    }

    // Resets all cauldron values
    internal void ResetValues()
    {
        cauldronFillMaterial.color = cauldronFillDefaultColor;
        possibleRecipes.Clear();
        possibleNextSteps.Clear();
        potionCompleted = false;
        potionIndex = 0;
        curStepIndex = 0;
        recipe = null;
        nextStep = null;
        cauldronFill.DOLocalMove(cauldronStartingPosition, 0.5f);
    }

    #region Stirring
    // Stir the cauldron clockwise
    private void StirClockwise(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            if (!canInteract) return;

            currentStep = "Stir_C_1";
            spoon.DOLocalRotate(new Vector3(0, 360, 16), spoonRotationSpeed, RotateMode.FastBeyond360);
            Stir();
        }
    }

    // Stir the cauldron counter-clockwise
    private void StirCounterClockwise(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            if (!canInteract) return;

            currentStep = "Stir_CC_1";
            spoon.DOLocalRotate(new Vector3(0, -360, 16), spoonRotationSpeed, RotateMode.FastBeyond360);
            Stir();
        }
    }
    #endregion

    // using this to check if the player is in range to stir the cauldron
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            canInteract = true;
            InputManager.OnStir?.Invoke();
        }
    }

    // using this to check if the player has left the range to stir the cauldron
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            canInteract = false;
            InputManager.OnHide?.Invoke();
        }
    }
}