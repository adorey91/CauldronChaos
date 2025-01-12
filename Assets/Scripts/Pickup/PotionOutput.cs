using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionOutput : MonoBehaviour, IPickupable
{
    public bool isPotionGood;
    public RecipeSO recipeGiven;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Pickup(InteractionDetector player)
    {
        rb.isKinematic = true;
        player.PickUpPotion(this.gameObject);
    }

    public void Drop(Transform newParent)
    {
        rb.isKinematic = false;
        transform.parent = newParent;
    }

    public bool AlreadyActive()
    {
        return true;
    }
}