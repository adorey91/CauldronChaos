using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeBookUI : MonoBehaviour
{
    [SerializeField] private RecipeManager recipeManager;
    [SerializeField] private GameObject leftPageHolder;
    [SerializeField] private GameObject rightPageHolder;

    private RecipeSO[] availableRecipes;

    [Header("Page Buttons")]
    [SerializeField] private GameObject previousPage;
    [SerializeField] private GameObject nextPage;

    [Header("Page Prefabs")]
    [SerializeField] private GameObject recipeUiPrefab;
    [SerializeField] private GameObject ingredientStepPrefab;

    private int _pageNumber = 0;
    private int _recipeNumber = 0;

    // Recipe UI Components
    private GameObject _newRecipeUI;
    private GameObject _ingredientHolderTop;
    private GameObject _ingredientHolderBottom;
    private GameObject _newIngredientUI;


    private void Start()
    {
        availableRecipes = recipeManager.FindAvailableRecipes();

        if (_pageNumber == 0)
            previousPage.SetActive(false);

        if (availableRecipes.Length < 4)
            nextPage.SetActive(false);

        SetRecipes();
    }

    // this will need to be reworked as currently it will only fill the right side.
    public void SetRecipes()
    {
        _recipeNumber = 0;
        for (int i = 0; i < 4; i++)
        {
            if (i > availableRecipes.Length - 1) return;
            // Instantiate new recipe UI
            _newRecipeUI = Instantiate(recipeUiPrefab, rightPageHolder.transform);

            Image imageComponent = _newRecipeUI.transform.GetChild(0).GetComponent<Image>();
            if (imageComponent != null)
                imageComponent.sprite = availableRecipes[_recipeNumber].potionIcon;
            else
                Debug.LogError("Image component not found in the children of _newRecipeUI!");


            _ingredientHolderBottom = _newRecipeUI.transform.Find("IngredientsHolderBottom").gameObject;
            _ingredientHolderTop = _newRecipeUI.transform.Find("IngredientsHolderTop").gameObject;

            CreateIngredientUI();

            _recipeNumber++;
        }
    }

    private void FillRightPage()
    {

    }

    private void FillLeftPage()
    {

    }


    private void CreateIngredientUI()
    {
        for (int recipeStep = 0; recipeStep < availableRecipes[_recipeNumber].steps.Length; recipeStep++)
        {
            if (recipeStep < 3)
                _newIngredientUI = Instantiate(ingredientStepPrefab, _ingredientHolderTop.transform);
            else
                _newIngredientUI = Instantiate(ingredientStepPrefab, _ingredientHolderBottom.transform);

            _newIngredientUI.GetComponent<Image>().sprite = availableRecipes[_recipeNumber].steps[recipeStep].ingredient.icon;
        }
    }
}
