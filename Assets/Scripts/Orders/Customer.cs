using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public RecipeSO order;
    public string customerName;

    public void AssignOrder(RecipeSO order)
    {
        Debug.Log("Customer assigned order: " + order.recipeName);
        this.order = order;
    }

    //public void OrderComplete(PotionOutput potionGiven)
    public void OrderComplete(PotionOutputTest potionGiven)
    {
        if (potionGiven.isPotionGood)
        {
            HandleSuccess();
        }
        else
        {
            HandleFailure();
        }
    }

    private void HandleFailure()
    {
        Debug.Log("Customer is angry");
    }

    private void HandleSuccess()
    {
        Debug.Log("Customer is happy");
    }
}