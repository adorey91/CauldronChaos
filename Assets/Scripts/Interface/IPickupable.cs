using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickupable 
{
    public void Pickup(InteractionDetector player);

    public void Drop(Transform newParent);

    public bool AlreadyActive();
}
