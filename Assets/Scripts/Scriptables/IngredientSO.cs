using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredient", menuName = "Recipe/Ingredient")]
public class IngredientSO : ScriptableObject
{
    public string ingredName;
}
