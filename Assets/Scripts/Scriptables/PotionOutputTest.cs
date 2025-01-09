using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PotionOutputTest", menuName = "Scriptables/PotionOutputTest")]
public class PotionOutputTest : ScriptableObject
{
    public bool isPotionBad = false;
    public RecipeSO recipeGiven;
}
