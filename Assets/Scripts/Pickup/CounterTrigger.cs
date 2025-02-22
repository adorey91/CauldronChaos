using UnityEngine;

public class CounterTrigger : MonoBehaviour
{
    [SerializeField] private Counter counter; //counter where the list of objects will be stored

    //Method called on collider entering the trigger volume
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Object added to counter");

        PickupObject pickup = other.GetComponent<PickupObject>();
        if (pickup != null)
        {
            counter.pickupObjects.Add(pickup); //add to the counter's list
        }
    }

    //Method called on collider leaving the trigger volume
    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("Object removed to counter");

        PickupObject pickup = other.GetComponent<PickupObject>();
        if (pickup != null)
        {
            counter.pickupObjects.Remove(pickup); //remove from the counter's list
        }
    }
}
