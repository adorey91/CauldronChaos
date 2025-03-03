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

    #region OnEnable / OnDisable / OnDestroy Events
    private void OnEnable()
    {
        InputManager.NextPageAction += NextPage;
        InputManager.PreviousPageAction += PreviousPage;
    }

    private void OnDisable()
    {
        InputManager.NextPageAction -= NextPage;
        InputManager.PreviousPageAction -= PreviousPage;
    }

    private void OnDestroy()
    {
        InputManager.NextPageAction -= NextPage;
        InputManager.PreviousPageAction -= PreviousPage;
    }
    #endregion


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
        _recipeUiLeft.potionIcon.preserveAspect = true;
        CreateIngredientUI(_recipeUiLeft, recipeIndex);
    }

    private void FillRightPage(int recipeIndex)
    {
        recipeUiRight.SetActive(true);
        _recipeUiRight.recipeName.text = availableRecipes[recipeIndex].recipeName;
        _recipeUiRight.potionIcon.sprite = availableRecipes[recipeIndex].potionIcon;
        _recipeUiRight.potionIcon.preserveAspect = true;
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
                TextMeshProUGUI stirText = recipeUI.recipeStepUI[i].GetComponentInChildren<TextMeshProUGUI>();

                Sprite stepSprite;

                if (availableRecipes[recipeIndex].steps[i].action == RecipeStepSO.ActionType.AddIngredient)
                {
                    stepSprite = availableRecipes[recipeIndex].steps[i].ingredientSprite;
                    stirText.enabled = false;
                }
                else
                {
                    stepSprite = availableRecipes[recipeIndex].steps[i].stirSprite;
                    stirText.enabled = true;
                    stirText.text = $"{availableRecipes[recipeIndex].steps[i].stirAmount}";
                }

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
        if (input.performed)
        {
            if ((_pageNumber + 1) * 2 < availableRecipes.Length)
            {
                _pageNumber++;
                SetRecipes();
            }
        }
    }

    public void PreviousPage(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            if (_pageNumber > 0)
            {
                _pageNumber--;
                SetRecipes();
            }
        }
    }

    public void ButtonNavigation(bool isNext)
    {
        if(isNext)
        {
            if ((_pageNumber + 1) * 2 < availableRecipes.Length)
            {
                _pageNumber++;
                SetRecipes();
            }
        }
        else
        {
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
}

[Serializable]
public class RecipeUI
{
    public TextMeshProUGUI recipeName;
    public Image potionIcon;
    public GameObject[] recipeStepUI;
}

