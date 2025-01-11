using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Recipe/Recipe")]
public class RecipeSO : ScriptableObject
{
    [Header("Recipe Name")]
    public string recipeName;

    [Header("Ingredient Steps / Input")]
    public RecipeStepSO[] steps;

    [Header("Potion / Output")]
    public GameObject potionPrefab;
    public Sprite potionIcon;
}
