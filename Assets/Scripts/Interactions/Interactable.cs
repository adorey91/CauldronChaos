using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private bool mustBePickedUp;
    private bool isPickedUp = false;

    //function that is a wrapper method for the interaction
    public void Interact()
    {
        //clause statement for requiring to be in player's hands and not being there
        if (mustBePickedUp && !isPickedUp)
        {
            return;
        }

        InteractFunction();
    }

    //function that stores the actual interaction functionality of the interactable
    public abstract void InteractFunction();

    public void TogglePickup()
    {
        isPickedUp = !isPickedUp;
    }
    
}
