using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderCounter : MonoBehaviour
{
    public void FillOrder(PotionOutput potion)
    {
        QueueManager.OnCheckCustomers?.Invoke(potion);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<PickupObject>(out PickupObject pickUp))
        {
            if (pickUp.isHeld)
                pickUp.Drop();
            if (other.TryGetComponent<PotionOutput>(out PotionOutput potion))
            {
                if (potion.givenToCustomer) return;

                FillOrder(potion);
            }
        }
    }
}
