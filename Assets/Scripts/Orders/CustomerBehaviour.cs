using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerBehaviour : MonoBehaviour
{
    public RecipeSO requestedOrder;
    public string customerName;
    private CustomTimer tipTimer;

    private bool giveTip;

    private Vector3 targetPosition;
    public float moveSpeed = 3f;
    private bool leavingQueue;

    private void Update()
    {
        //if(tipTimer.UpdateTimer())
        //    giveTip = false;

        if(!leavingQueue)
            MoveToTarget();
    }


    #region Order Events

    public void AssignOrder(RecipeSO order)
    {
        Debug.Log("Customer assigned order: " + order.recipeName);
        this.requestedOrder = order;
        tipTimer = new CustomTimer(2, true);
        giveTip = true;
    }

    public void OrderComplete(PotionOutput potionGiven)
    {
        if (giveTip == true)
        {
            Debug.Log("Customer is happy");
            Actions.OnCustomerServed?.Invoke(true);
        }
        else
        {
            Debug.Log("Customer is okay");
            Actions.OnCustomerServed?.Invoke(false);
        }
    }
    #endregion

    #region Positioning
    public void SetTarget(Vector3 position)
    {
        targetPosition = position;
        leavingQueue = false;
    }

    // Leave the queue and call a callback once finished
    public void LeaveQueue(Vector3 exitPosition, System.Action onExitComplete)
    {
        leavingQueue = true;
        StartCoroutine(LeaveAndExit(exitPosition, onExitComplete));
    }

    private void MoveToTarget()
    {
        if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, 270, 0); 
        }
        else
            transform.rotation = Quaternion.identity;
    }

    private IEnumerator LeaveAndExit(Vector3 exitPosition, System.Action onExitComplete)
    {
        float stepDistance = 1.2f; // Distance to step back
        Vector3 backwardStep = transform.position - transform.forward * stepDistance; // Step back 1 unit

        transform.rotation = Quaternion.Euler(0, 180, 0); 

        // Step 1: Move 1 unit backward
        while (Vector3.Distance(transform.position, backwardStep) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, backwardStep, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.rotation = Quaternion.Euler(0, 90, 0); // Rotate -90 degrees

        // Step 3: Move to the exit
        while (Vector3.Distance(transform.position, exitPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, exitPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Notify that the customer has left
        onExitComplete?.Invoke();
    }

    #endregion
}
