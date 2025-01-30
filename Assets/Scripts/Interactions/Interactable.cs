using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Interactable : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private bool mustBePickedUp;

    //function that stores the actual interaction functionality of the interactable
    public abstract void Interact();
    
    //Accessor method that returns if the iteractable must be picked-up
    public bool MustBePickedUp()
    {
        return mustBePickedUp;
    }
}
