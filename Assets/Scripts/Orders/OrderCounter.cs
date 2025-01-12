using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderCounter : MonoBehaviour, IInteractable
{
    private OrderManager _orderManager;

    [SerializeField] private GameObject startingPosition;
    private Transform firstPosition;
    [SerializeField] private int maxCustomers = 5;
    [SerializeField] private float positionSize = 1;
    List<Vector3> waitingQueuePosition = new();
    private int current = 0;
    private bool[] positionOccupied;

    private void Start()
    {
        _orderManager = FindObjectOfType<OrderManager>();
        firstPosition = startingPosition.transform;

        positionOccupied = new bool[maxCustomers];

        for (int i = 0; i < maxCustomers; i++)
        {
            waitingQueuePosition.Add(firstPosition.position + new Vector3(-i, 0, 0) * positionSize);
            positionOccupied[i] = false; // All positions are initially empty
        }
    }


    public void Interact(InteractionDetector player)
    {
        if (!player.HasPotion()) return;

        PotionOutput output = player.GetPotion().GetComponent<PotionOutput>();

        _orderManager.FinishOrder(output);
        Actions.OnRemovePotion?.Invoke();
    }

    public Vector3 GetNextPosition()
    {
        for (int i = 0; i < waitingQueuePosition.Count; i++)
        {
            if (!positionOccupied[i]) // Find the first empty spot
            {
                positionOccupied[i] = true; // Mark this position as occupied
                return waitingQueuePosition[i];
            }
        }

        Debug.LogWarning("No available positions in the queue!");
        return Vector3.zero; // Return a default position if all spots are full
    }

    public void FreePosition(Vector3 position)
    {
        for (int i = 0; i < waitingQueuePosition.Count; i++)
        {
            if (waitingQueuePosition[i] == position)
            {
                positionOccupied[i] = false; // Mark the position as empty
                return;
            }
        }

        Debug.LogWarning("Tried to free a position that is not part of the queue.");
    }

    public Transform ParentPosition()
    {
        return firstPosition;
    }
}
