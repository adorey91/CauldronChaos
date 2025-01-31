using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderCounter : MonoBehaviour
{
    [SerializeField] private GameObject startingPosition;
    private Transform firstPosition;
    [SerializeField] private int maxCustomers = 5;
    [SerializeField] private float positionSize = 1;
    List<Vector3> waitingQueuePosition = new();
    private bool[] positionOccupied;

    private void Start()
    {
        firstPosition = startingPosition.transform;

        positionOccupied = new bool[maxCustomers];

        for (int i = 0; i < maxCustomers; i++)
        {
            waitingQueuePosition.Add(firstPosition.position + new Vector3(i, 0, 0) * positionSize);
            positionOccupied[i] = false; // All positions are initially empty
        }
    }

    private void OnEnable()
    {
        Actions.OnEndDay += ResetCounter;
    }

    private void OnDisable()
    {
        Actions.OnEndDay -= ResetCounter;
    }

    public void FillOrder(PotionOutput potion)
    {
        QueueManager.OnCheckCustomers?.Invoke(potion);
    }

    private void ResetCounter()
    {
        for (int i = 0; i < maxCustomers; i++)
        {
            positionOccupied[i] = false; // All positions are initially empty
        }
    }

    public Transform ParentPosition()
    {
        return firstPosition;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<PickupObject>(out PickupObject pickUp))
        {
            if (pickUp.isHeld) return;
            if (other.TryGetComponent<PotionOutput>(out PotionOutput potion))
            {
                if (potion.givenToCustomer) return;

                FillOrder(potion);
            }
        }
    }
}
