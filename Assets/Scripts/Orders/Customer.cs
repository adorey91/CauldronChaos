using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public RecipeSO order;
    public string customerName;

    public void AssignOrder(RecipeSO order)
    {
        this.order = order;
    }

    public void OrderComplete(RecipeSO recipeGiven)
    {
        if(recipeGiven == order)
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
