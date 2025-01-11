using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientHolder : MonoBehaviour
{
    // Holds the ingredientBeingHeld for the prefab
    public IngredientSO ingredient;

    internal void TurnOff()
    {
        this.gameObject.SetActive(false);
    }
}
