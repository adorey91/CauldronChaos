using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionOutput : MonoBehaviour, IPickupable
{
    public bool isPotionGood;
    public RecipeSO recipeGiven;
    private Rigidbody _rb;
    private Collider _collider;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Pickup(InteractionDetector player)
    {
        _rb.isKinematic = true;
        _collider.enabled = false;
        player.PickUpPotion(this.gameObject);
    }

    public void Drop(Transform newParent)
    {
        _rb.isKinematic = false;
        _collider.enabled = true;
        transform.parent = newParent;
    }

    public bool AlreadyActive()
    {
        return true;
    }
}