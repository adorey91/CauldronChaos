using UnityEngine;
using System;

public class OrderCounter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PickupObject>(out PickupObject pickUp))
        {
            if (other.TryGetComponent<PotionOutput>(out PotionOutput potion))
            {
                if (potion.givenToCustomer) return;

                Actions.FilledOrder?.Invoke();
                Actions.OnCheckCustomers?.Invoke(potion);
            }
        }
    }
}
