using UnityEngine;
using System;

public class OrderCounter : MonoBehaviour
{
    public static Action FilledOrder;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PickupObject>(out PickupObject pickUp))
        {
            if (other.TryGetComponent<PotionOutput>(out PotionOutput potion))
            {
                if (potion.givenToCustomer) return;

                FilledOrder?.Invoke();
                QueueManager.OnCheckCustomers?.Invoke(potion);
            }
        }
    }
}
