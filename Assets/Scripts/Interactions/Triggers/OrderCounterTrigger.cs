using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderCounterTrigger : MonoBehaviour
{
    [SerializeField] private OrderCounter orderCounter;

    private void OnTriggerEnter(Collider other)
    {
        PotionOutput output = other.GetComponent<PotionOutput>();
        if (output != null)
        {
            orderCounter.FillOrder(output);
        }
    }
}
