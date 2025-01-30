using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bucket : Interactable
{
    public override void Interact()
    {
        Debug.Log("Bucket interacted with");
    }

    public override void Interact(PickupBehaviour pickup)
    {
        throw new System.NotImplementedException();
    }
}
