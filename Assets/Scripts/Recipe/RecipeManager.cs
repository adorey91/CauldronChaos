using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    [Header("Available Recipes")]
    [SerializeField] private RecipeSO[] allRecipes;
    [SerializeField] private int numberOfRecipes;

    [SerializeField] private GameObject recipeBookUi;

    [SerializeField] private bool useAllRecipes;
    [SerializeField] private GameObject closeButton;

    private void Update()
    {
        if(recipeBookUi.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleRecipeBook();
            }
        }
    }

    #region OnEnable / OnDisable / OnDestroy Events
    private void OnEnable()
    {
        Actions.OnToggleRecipeBook += ToggleRecipeBook;
    }

    private void OnDisable()
    {
        Actions.OnToggleRecipeBook -= ToggleRecipeBook;
    }

    private void OnDestroy()
    {
        Actions.OnToggleRecipeBook -= ToggleRecipeBook;
    }
    #endregion

    public RecipeSO[] FindAvailableRecipes()
    {
        RecipeSO[] availableRecipes = new RecipeSO[numberOfRecipes];

        if (useAllRecipes)
        {
            availableRecipes = allRecipes;
        }
        else
        {
            for (int i = 0; i < numberOfRecipes; i++)
            {
                availableRecipes[i] = allRecipes[i];
            }
        }


        return availableRecipes;
    }

    internal RecipeSO GetRandomRecipe()
    {
        return allRecipes[Random.Range(0, numberOfRecipes)];
    }

    internal RecipeSO GetRecipeByName(string recipeName)
    {
        foreach (RecipeSO recipe in allRecipes)
        {
            if (recipe.recipeName == recipeName)
                return recipe;
        }
        return null;
    }

    public void ToggleRecipeBook()
    {

        if (recipeBookUi.activeSelf)
        {
            recipeBookUi.SetActive(false);
           InputManager.OnGameplayInputs();
        }
        else
        {
            recipeBookUi.SetActive(true);
            Actions.OnSelectRecipeButton(closeButton);
            InputManager.OnRecipeBookInputs();
        }
    }
}
