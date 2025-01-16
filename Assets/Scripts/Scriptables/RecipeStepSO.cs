using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Step", menuName = "Recipe/Recipe Step")]
public class RecipeStepSO : ScriptableObject
{
    public enum StepType { Nothing, AddIngredient , StirClockwise, StirCounterClockwise }
    public StepType stepType;
    
    // shows if using stir
    public Sprite instructionSprite;
    
    public int stirAmount; // Optional, used if the step is stirring
    public IngredientSO ingredient; // Optional, used if the step is adding an ingredientBeingHeld

    // Ingredient
    public enum IngredientType { bottle, mushroom, eye}
    public IngredientType ingredient1;
    public Sprite sprite;
}
