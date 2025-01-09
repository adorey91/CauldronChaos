using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Step", menuName = "Recipe/Recipe Step")]
public class RecipeStepSO : ScriptableObject
{
    public enum StepType { AddIngredient , StirClockwise, StirCounterClockwise }
    public StepType stepType;
    public IngredientSO ingredient; // Optional, used if the step is adding an ingredient
    public int stirAmount; // Optional, used if the step is stirring
}
