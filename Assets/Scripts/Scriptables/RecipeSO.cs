using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Recipe/Recipe")]
public class RecipeSO : ScriptableObject
{
    public IngredientSO[] ingredient;
}
