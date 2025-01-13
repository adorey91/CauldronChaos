using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientHolder : MonoBehaviour, IPickupable
{
    // Holds the ingredientBeingHeld for the prefab
    public IngredientSO ingredient;

    public bool AlreadyActive()
    {
        return true;
    }

    public void Pickup(InteractionDetector player)
    {
        GetComponent<Rigidbody>().isKinematic = true;
        player.PickUpIngredient(this.gameObject);
    }

    public void Drop(Transform newParent)
    {
        transform.parent = newParent;
        GetComponent<Rigidbody>().isKinematic = false;
    }
}
