using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateHolder : MonoBehaviour, IInteractable
{
   public GameObject ingredientPrefab;

    public void Interact(InteractionDetector player)
    {
        if(player.HasIngredient() || player.HasPotion()) return;

        player.AddIngredient(ingredientPrefab);
    }
}
