using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public RecipeSO order;
    public string customerName;
    private CustomTimer tipTimer;

    private bool giveTip;

    private void Update()
    {
        if (tipTimer.UpdateTimer())
        {
            giveTip = false;
        }
    }

    /// <summary>
    /// Assigns customer order, starts tip timer
    /// </summary>
    public void AssignOrder(RecipeSO order)
    {
        Debug.Log("Customer assigned order: " + order.recipeName);
        this.order = order;
        tipTimer = new CustomTimer(2, true);
        giveTip = true;
    }

    // If order is completed
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
}