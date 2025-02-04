using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class CauldronRecipe : MonoBehaviour
{
    private RecipeManager recipeManager;

    private RecipeSO[] craftableRecipes;
    private List<RecipeSO> possibleRecipes = new();

    private RecipeSO recipe;
    private RecipeStepSO nextStep;

    private int stirCount;
    private int stepIndex;

    private bool addedRecipe;

    [SerializeField] private VisualEffect incorrectStep;

    public void Start()
    {
        recipeManager = GetComponent<RecipeManager>();
        craftableRecipes = recipeManager.FindAvailableRecipes();
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {

    }

    internal void AddIngredient()
    {

    }

    internal void Stir(bool isClockwise)
    {

    }

}
