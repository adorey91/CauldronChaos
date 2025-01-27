using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Step", menuName = "Recipe/Recipe Step")]
public class RecipeStepSO : ScriptableObject
{
    public enum StepType { Nothing, AddIngredient , StirClockwise, StirCounterClockwise }
    public StepType stepType;
    
    // shows if using stir
    public Sprite stirSprite;
    public int stirAmount; // Optional, used if the step is stirring

    // Ingredient
    public IngredientSO ingredient;
    public Sprite ingredientSprite;
}
