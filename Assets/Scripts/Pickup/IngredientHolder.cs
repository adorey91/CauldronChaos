using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientHolder : MonoBehaviour, IPickupable
{
    // Holds the ingredientBeingHeld for the prefab
    public RecipeStepSO recipeStepIngredient;
    private Rigidbody _rb;
    private Collider _collider;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    public bool AlreadyActive()
    {
        return true;
    }

    public void Pickup(InteractionDetector player)
    {
        _rb.isKinematic = true;
        _collider.enabled = false;
        player.PickUpIngredient(this.gameObject);
    }

    public void Drop(Transform newParent)
    {
        transform.parent = newParent;
        _collider.enabled = true;
        _rb.isKinematic = false;
    }
}
