using System.Collections.Generic;
using UnityEngine;

public class Counter : Interactable
{
    public List<PickupObject> pickupObjects; //list of all pickup objects on counter

    //Unimplemented regular interact function
    public override void Interact()
    {
        throw new System.NotImplementedException();
    }

    //Function that holds interact functionality for getting object off of counter
    public override void Interact(PickupBehaviour playerPickup)
    {
        Debug.Log("Counter Interact called");

        //Exit function  if nothing is on counter is selected
        if (pickupObjects.Count <= 0)
        {
            Debug.Log("Nothing on Counter");
            return;
        }

        playerPickup.SetHeldObject(pickupObjects[0]); //manually add object to player hands
    }
}
