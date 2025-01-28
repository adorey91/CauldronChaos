using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RecipeBookUI : MonoBehaviour
{
    [SerializeField] private RecipeManager recipeManager;
    [SerializeField] private GameObject recipeUiLeft;
    [SerializeField] private GameObject recipeUiRight;

    private RecipeSO[] availableRecipes;

    [Header("Page Buttons")]
    [SerializeField] private GameObject previousPage;
    [SerializeField] private GameObject nextPage;

    private int _pageNumber = 0;

    // Recipe UI Components
    [SerializeField] private RecipeUI _recipeUiLeft;
    [SerializeField] private RecipeUI _recipeUiRight;

    private void Start()
    {
        availableRecipes = recipeManager.FindAvailableRecipes();

        if (_pageNumber == 0)
            previousPage.SetActive(false);

        if (availableRecipes.Length - 1 < 2)
            nextPage.SetActive(false);

        SetRecipes();
    }

    private void OnEnable()
    {
        InputManager.instance.NextPageAction += NextPage;
        InputManager.instance.PreviousPageAction += PreviousPage;
    }

    private void OnDisable()
    {
        InputManager.instance.NextPageAction -= NextPage;
        InputManager.instance.PreviousPageAction -= PreviousPage;
    }

    public void SetRecipes()
    {
        ClearPage(); // Clear current UI elements

        int firstRecipeIndex = _pageNumber * 2; // First recipe on the current screen
        int secondRecipeIndex = firstRecipeIndex + 1; // Second recipe on the current screen

        // Fill left page if a recipe exists
        if (firstRecipeIndex < availableRecipes.Length)
        {
            FillLeftPage(firstRecipeIndex);
        }

        // Fill right page if a recipe exists
        if (secondRecipeIndex < availableRecipes.Length)
        {
            FillRightPage(secondRecipeIndex);
        }

        UpdatePageButtons();
    }


    private void FillLeftPage(int recipeIndex)
    {
        recipeUiLeft.SetActive(true);
        _recipeUiLeft.recipeName.text = availableRecipes[recipeIndex].recipeName;
        _recipeUiLeft.potionIcon.sprite = availableRecipes[recipeIndex].potionIcon;
        CreateIngredientUI(_recipeUiLeft, recipeIndex);
    }

    private void FillRightPage(int recipeIndex)
    {
        recipeUiRight.SetActive(true);
        _recipeUiRight.recipeName.text = availableRecipes[recipeIndex].recipeName;
        _recipeUiRight.potionIcon.sprite = availableRecipes[recipeIndex].potionIcon;
        CreateIngredientUI(_recipeUiRight, recipeIndex);
    }

    private void ClearPage()
    {
        recipeUiRight.SetActive(false);
        recipeUiLeft.SetActive(false);
    }

    private void CreateIngredientUI(RecipeUI recipeUI, int recipeIndex)
    {
        int totalSteps = availableRecipes[recipeIndex].steps.Length;

        for (int i = 0; i < recipeUI.recipeStepUI.Length; i++)
        {
            if (i < totalSteps)
            {
                recipeUI.recipeStepUI[i].SetActive(true);
                var stepSprite = availableRecipes[recipeIndex].steps[i].ingredientSprite;
                var stepImage = recipeUI.recipeStepUI[i].GetComponent<Image>();
                stepImage.sprite = stepSprite;
                stepImage.preserveAspect = true;
            }
            else
            {
                recipeUI.recipeStepUI[i].SetActive(false);
            }
        }
    }

    #region Navigation
    public void NextPage(InputAction.CallbackContext input)
    {
        if(input.performed)
        {

            Debug.Log("nextpage");
            if ((_pageNumber + 1) * 2 < availableRecipes.Length)
            {
                _pageNumber++;
                SetRecipes();
            }
        }
    }

    public void PreviousPage(InputAction.CallbackContext input)
    {
        if(input.performed)
        {
            Debug.Log("Previous Page");
            if (_pageNumber > 0)
            {
                _pageNumber--;
                SetRecipes();
            }
        }
    }

    private void UpdatePageButtons()
    {
        previousPage.SetActive(_pageNumber > 0);
        nextPage.SetActive((_pageNumber + 1) * 2 < availableRecipes.Length);
    }
    #endregion

    private IEnumerator PageDelay()
    {
        yield return new WaitForSeconds(0.4f);
    }
}

[Serializable]
public class RecipeUI
{
    public TextMeshProUGUI recipeName;
    public Image potionIcon;
    public GameObject[] recipeStepUI;
}

