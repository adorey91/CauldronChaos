using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RecipeBookUI : MonoBehaviour
{
    [SerializeField] private RecipeManager recipeManager;
    [SerializeField] private GameObject recipeUiLeftObj;
    [SerializeField] private GameObject recipeUiRightObj;

    private RecipeSO[] _availableRecipes;

    [Header("Page Buttons")]
    [SerializeField] private GameObject previousPage;
    [SerializeField] private GameObject nextPage;

    private int _pageNumber;

    // Recipe UI Components
    [SerializeField] private RecipeUI recipeUiLeft;
    [SerializeField] private RecipeUI recipeUiRight;

    private void Start()
    {
        _availableRecipes = recipeManager.FindAvailableRecipes();

        if (_pageNumber == 0)
            previousPage.SetActive(false);

        if (_availableRecipes.Length - 1 < 2)
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


    private void SetRecipes()
    {
        ClearPage(); // Clear current UI elements

        int firstRecipeIndex = _pageNumber * 2; // First recipe on the current screen
        int secondRecipeIndex = firstRecipeIndex + 1; // Second recipe on the current screen

        // Fill left page if a recipe exists
        if (firstRecipeIndex < _availableRecipes.Length)
        {
            FillLeftPage(firstRecipeIndex);
        }

        // Fill right page if a recipe exists
        if (secondRecipeIndex < _availableRecipes.Length)
        {
            FillRightPage(secondRecipeIndex);
        }

        UpdatePageButtons();
    }


    private void FillLeftPage(int recipeIndex)
    {
        recipeUiLeftObj.SetActive(true);
        recipeUiLeft.recipeName.text = _availableRecipes[recipeIndex].recipeName;
        recipeUiLeft.potionIcon.sprite = _availableRecipes[recipeIndex].potionIcon;
        recipeUiLeft.potionIcon.preserveAspect = true;
        recipeUiLeft.potionPrice.text = _availableRecipes[recipeIndex].sellAmount.ToString();
        CreateIngredientUI(recipeUiLeft, recipeIndex);
    }

    private void FillRightPage(int recipeIndex)
    {
        recipeUiRightObj.SetActive(true);
        recipeUiRight.recipeName.text = _availableRecipes[recipeIndex].recipeName;
        recipeUiRight.potionIcon.sprite = _availableRecipes[recipeIndex].potionIcon;
        recipeUiRight.potionIcon.preserveAspect = true;
        recipeUiRight.potionPrice.text = _availableRecipes[recipeIndex].sellAmount.ToString();
        CreateIngredientUI(recipeUiRight, recipeIndex);
    }

    private void ClearPage()
    {
        recipeUiRightObj.SetActive(false);
        recipeUiLeftObj.SetActive(false);
    }

    private void CreateIngredientUI(RecipeUI recipeUI, int recipeIndex)
    {
        int totalSteps = _availableRecipes[recipeIndex].steps.Length;

        for (int i = 0; i < recipeUI.recipeStepUI.Length; i++)
        {
            if (i < totalSteps)
            {
                recipeUI.recipeStepUI[i].SetActive(true);
                TextMeshProUGUI stirText = recipeUI.recipeStepUI[i].GetComponentInChildren<TextMeshProUGUI>();

                Sprite stepSprite;

                if (_availableRecipes[recipeIndex].steps[i].action == RecipeStepSO.ActionType.AddIngredient)
                {
                    stepSprite = _availableRecipes[recipeIndex].steps[i].ingredientSprite;
                    stirText.enabled = false;
                }
                else
                {
                    stepSprite = _availableRecipes[recipeIndex].steps[i].stirSprite;
                    stirText.enabled = true;
                    stirText.text = $"{_availableRecipes[recipeIndex].steps[i].stirAmount}";
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

    private void NextPage(InputAction.CallbackContext input)
    {
        if (!input.performed) return;
        if ((_pageNumber + 1) * 2 < _availableRecipes.Length)
        {
            _pageNumber++;
            SetRecipes();
        }
    }

    private void PreviousPage(InputAction.CallbackContext input)
    {
        if (!input.performed) return;
        if (_pageNumber > 0)
        {
            _pageNumber--;
            SetRecipes();
        }
    }

    public void ButtonNavigation(bool isNext)
    {
        if(isNext)
        {
            if ((_pageNumber + 1) * 2 < _availableRecipes.Length)
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
        nextPage.SetActive((_pageNumber + 1) * 2 < _availableRecipes.Length);
    }
    #endregion
}

[Serializable]
public class RecipeUI
{
    public TextMeshProUGUI recipeName;
    public Image potionIcon;
    public TextMeshProUGUI potionPrice;
    public GameObject[] recipeStepUI;
}

