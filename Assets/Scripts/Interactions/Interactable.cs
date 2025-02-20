using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public abstract class Interactable : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private bool mustBePickedUp;
    [SerializeField] private bool isContainer;

    //function that stores the actual interaction functionality of the interactable
    public abstract void Interact();

    //function that is an overload method of the interact funtion for the use of crates
    public abstract void Interact(PickupBehaviour pickup);
    
    //Accessor method that returns if the iteractable must be picked-up
    public bool MustBePickedUp()
    {
        return mustBePickedUp;
    }

    //Accessor method that returns if the interactable is a crate
    public bool IsContainer()
    {
        return isContainer;
    }
}
