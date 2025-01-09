using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour
{
    [Header("Order Variables")]
    [SerializeField] private RecipeManager recipeManager;
    [SerializeField] private int maxOrders = 5;
    private RecipeSO[] availableRecipes;

    [Header("Customer Prefab")]
    [SerializeField] GameObject[] customerPrefab;

    [Header("UI Variables")]
    [SerializeField] private GameObject orderUiHolder;
    [SerializeField] private GameObject orderUiPrefab;

    [Header("Active Customers")]
    private List<CustomerOrder> _activeOrders = new();

    // Timer for the day - sets 5 minute timer
    [Header("Day Timer Settings")]
    private CustomTimer dayTimer = new(5);
    [SerializeField] private TextMeshProUGUI dayTimerText;

    // Customer Order Timer
    private CustomTimer newCustomerTimer = new(0.1f);

    [Header("Testing Only")]
    // used for testing
    bool finished;
    [SerializeField] private PotionOutputTest[] potion;
    int potionIndex = 0;
    bool startCustomer = false;


    private void Start()
    {
        availableRecipes = recipeManager.FindAvailableRecipes();
        _activeOrders.Clear();
    }

    // All input.getkeys are testing imputs. They will be removed later.
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            // starts the "work day" timer
            dayTimer.StartTimer();
        }

        if (dayTimer.UpdateTimer())
        {
            Debug.Log("Day is over");
            startCustomer = false;
        }
        else
        {
            float remainingTime = dayTimer.GetRemainingTime();
            // Convert remaining time into minutes and seconds
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);

            dayTimerText.text = $"{minutes:00}:{seconds:00}";
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            startCustomer = true;
            Debug.Log("Customer Started");
            newCustomerTimer.StartTimer();
        }

        if(startCustomer)
        {
            if (_activeOrders.Count < maxOrders)
            {
                if (newCustomerTimer.UpdateTimer())
                {
                    GenerateOrder();
                    newCustomerTimer.ResetTimer();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            potionIndex = 0;
            finished = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            potionIndex = 1;
            finished = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            potionIndex = 2;
            finished = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            potionIndex = 3;
            finished = true;
        }

        if (finished)
        {
            FinishOrder(potion[potionIndex]);
            finished = false;
        }
    }

    // Generate a random order for a customer
    private void GenerateOrder()
    {
        // If there are no recipes, do nothing
        if (availableRecipes.Length == 0) return;

        GameObject customerObject = Instantiate(customerPrefab[Random.Range(0, customerPrefab.Length)]);
        Customer customer = customerObject.GetComponent<Customer>();
        RecipeSO assignedOrder;

        if (customer.customerName == "EvilMage")
        {
            assignedOrder = availableRecipes[0];
        }
        else
        {
            int randomIndex = Random.Range(0, availableRecipes.Length);
            assignedOrder = availableRecipes[randomIndex];
        }

        GameObject orderUi = Instantiate(orderUiPrefab, orderUiHolder.transform);
        orderUi.GetComponent<Image>().sprite = assignedOrder.potionIcon;
        orderUi.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = assignedOrder.recipeName;

        // Add the order to the active orders list
        _activeOrders.Add(new CustomerOrder
        {
            Customer = customer,
            OrderUi = orderUi,
            Recipe = assignedOrder
        });
    }


    //private void FinishOrder(PotionOutput recipe)
    private void FinishOrder(PotionOutputTest recipe)
    {
        // If there are no active orders, do nothing
        if (_activeOrders.Count == 0)
        {
            Debug.Log("No customers to serve");
            return;
        }

        //Check if the recipe matches any of the current orders
        foreach (var order in _activeOrders)
        {
            if (order.Recipe == recipe.recipeGiven)
            {
                order.Customer.OrderComplete(recipe);
                Destroy(order.Customer.gameObject);
                Destroy(order.OrderUi);

                _activeOrders.Remove(order);
                return;
            }
            else
            {
                Debug.Log("No customer found with that order");
            }
        }
    }
}