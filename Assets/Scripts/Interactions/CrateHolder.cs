using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateHolder : MonoBehaviour, IPickupable
{
   public GameObject ingredientPrefab;

    public bool AlreadyActive()
    {
        return false;
    }

    public void Drop(Transform newParent)
    {
        return;
    }
    
    public void Pickup(InteractionDetector player)
    {
        if (player.HasIngredient() || player.HasPotion()) return;

        player.AddIngredient(ingredientPrefab);
    }
}
