using System;
using UnityEngine;

[Serializable]
public class CustomerOrder
{
    public Customer Customer;
    public GameObject OrderUi;
    public GameObject CustomerGameObject;
    public RecipeSO Recipe;
}