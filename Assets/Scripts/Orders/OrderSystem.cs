using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderSystem : MonoBehaviour
{
    [Header("Find Recipes")]
    [SerializeField] private RecipeManager recipeManager;
    private RecipeSO[] availableRecipes;

    [Header("Customer Prefab")]
    [SerializeField] GameObject[] customerPrefab;
    [SerializeField] List<Customer> currentCustomers;


    private void Start()
    {
        availableRecipes = recipeManager.FindAvailableRecipes();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            GenerateOrder();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(FinishOrder(availableRecipes[0]));
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCoroutine(FinishOrder(availableRecipes[1]));
        }
    }

    private void GenerateOrder()
    {
        if (availableRecipes.Length == 0) return;

        int customerIndex = Random.Range(0, customerPrefab.Length);
        Instantiate(customerPrefab[customerIndex]);

        Customer customerScript = customerPrefab[customerIndex].GetComponent<Customer>();
        currentCustomers.Add(customerScript);

        if (customerScript.customerName == "Evil Mage")
        {
            foreach (var item in availableRecipes)
            {
                if (item.recipeName == "Potion of Hydration")
                {
                    customerScript.AssignOrder(item);
                    return;
                }
            }
        }
        else
        {
            int recipeIndex = Random.Range(0, availableRecipes.Length - 1);
            customerScript.AssignOrder(availableRecipes[recipeIndex]);
        }
    }


    private IEnumerator FinishOrder(RecipeSO recipe)
    {
        foreach (var customer in currentCustomers)
        {
            if (recipe == customer.order)
            {
                customer.OrderComplete(recipe);
                yield return new WaitForSeconds(1f);
                customer.gameObject.SetActive(false);
                currentCustomers.Remove(customer);

            }
        }
    }
}
