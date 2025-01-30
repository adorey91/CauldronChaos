using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionDetection : MonoBehaviour
{
    private List<Interactable> interactables = new List<Interactable>(); //list of interactables within the interactable space

    //Function that is called when collider enters the trigger volume
    private void OnTriggerEnter(Collider other)
    {
        //try to get interactable component
        Interactable interactable = other.GetComponent<Interactable>();

        //if component exists add to list
        if (interactable != null)
        {
            interactables.Add(interactable);
        }
    }

    //Function that is called when a collider exits the trigger volume
    private void OnTriggerExit(Collider other)
    {
        //try to get interactable component
        Interactable interactable = other.GetComponent<Interactable>();

        //if component exists add to list
        if (interactable != null)
        {
            interactables.Remove(interactable);
        }
    }

    //Function that returns the first interactable in the list that do not require being held
    public Interactable GetFirstNonHeldInteractable()
    {
        //loop through interactables list
        for (int i=0; i<interactables.Count; i++)
        {
            //if does not require being picked up for use return
            if (!interactables[i].MustBePickedUp())
            {
                return interactables[i];
            }
        }

        return null; //return null if nothing was found
    }
}
