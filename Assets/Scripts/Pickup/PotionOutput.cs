using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionOutput : MonoBehaviour, IPickupable
{
    public RecipeSO potionInside;
    private Rigidbody _rb;
    private Collider _collider;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    public void Pickup(InteractionDetector player)
    {
        _rb.isKinematic = true;
        _collider.enabled = false;
        player.PickUpPotion(this.gameObject);
    }

    public void Drop(Transform newParent)
    {
        if (newParent == null)
            transform.parent = null;
        else
            transform.parent = newParent;

        _rb.isKinematic = false;
        _collider.enabled = true;
    }

    public bool AlreadyActive()
    {
        return true;
    }
}