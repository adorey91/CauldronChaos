using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Step", menuName = "Recipe/Recipe Step")]
public class RecipeStepSO : ScriptableObject
{
    public enum ActionType { Nothing, AddIngredient, Stir }
    public ActionType action;

    public string stepName;

    // shows if using stir
    public Sprite stirSprite;
    public bool isClockwise; // Optional, used if the step is stirring
    public int stirAmount; // Optional, used if the step is stirring

    // Ingredient
    public enum Ingredient { Nothing, Bottle, Mushroom, Eye_of_Basilisk, Rabbit_Foot, Troll_Bone , Mandrake_Root };
    public Ingredient ingredient;
    public Sprite ingredientSprite;
}
