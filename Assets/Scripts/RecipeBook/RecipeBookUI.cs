using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
    private int _recipeNumber = 0;

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

    // this will need to be reworked as currently it will only fill the right side.
    public void SetRecipes()
    {
        _recipeNumber = 0;

        for (int i = 0; i < 2; i++)
         {
            if (i > availableRecipes.Length - 1) return;

            if (i == 0)
                FillLeftPage();
            else
                FillRightPage();

            _recipeNumber++;
        }
    }

    private void FillRightPage()
    {
        recipeUiRight.SetActive(true);
        _recipeUiRight.recipeName.text = availableRecipes[_recipeNumber].recipeName;
        _recipeUiRight.potionIcon.sprite = availableRecipes[_recipeNumber].potionIcon;

        CreateIngredientUI(_recipeUiRight);
    }

    private void FillLeftPage()
    {
        recipeUiLeft.SetActive(true);
        _recipeUiLeft.recipeName.text = availableRecipes[_recipeNumber].recipeName;
        _recipeUiLeft.potionIcon.sprite = availableRecipes[_recipeNumber].potionIcon;
        CreateIngredientUI(_recipeUiLeft);
    }


    private void CreateIngredientUI(RecipeUI side)
    {
        int totalSteps = availableRecipes[_recipeNumber].steps.Length;

        // Loop through all UI elements
        for (int i = 0; i < side.recipeStepUI.Length; i++)
        {
            // If the current index is within the number of steps, activate and update text
            if (i < totalSteps)
            {
                side.recipeStepUI[i].SetActive(true);
                side.recipeStepUI[i].GetComponentInChildren<TextMeshProUGUI>().text = $"{i + 1}";
            }
            // Otherwise, deactivate the UI element
            else
            {
                side.recipeStepUI[i].SetActive(false);
            }
        }

    }
}

[Serializable]
public class RecipeUI
{
    public TextMeshProUGUI recipeName;
    public Image potionIcon;
    public GameObject[] recipeStepUI;
}

