using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour
{
    [Header("Order Variables")]
    [SerializeField] private RecipeManager recipeManager;
    [SerializeField] private int maxOrders = 4;
    private RecipeSO[] _availableRecipes;

    [Header("Order UI")]
    [SerializeField] private OrderManagerUi orderManagerUi;

    [Header("Customer Prefab")]
    [SerializeField] GameObject[] customerPrefab;

    [Header("Active Customers")]
    private List<CustomerOrder> _activeOrders = new();

    // Timer for the day - sets 5 minute timer
    [Header("Day Timer Settings")]
    [SerializeField] private int minutesPerDay = 5;
    [SerializeField] private TextMeshProUGUI dayTimerText;
    private CustomTimer _dayTimer;
    private bool _startCustomer = false;

    // Customer Order Timer - this time is in seconds
    [Header("Customer Timer Settings")]
    [SerializeField] private int newCustomerTime = 6;
    private CustomTimer _newCustomerTimer;

    private void Start()
    {
        recipeManager = FindObjectOfType<RecipeManager>();
        _dayTimer = new CustomTimer(minutesPerDay, true);
        _newCustomerTimer = new CustomTimer(newCustomerTime, false);
        _availableRecipes = recipeManager.FindAvailableRecipes();
        _activeOrders.Clear();
    }

    private void OnEnable()
    {
        Actions.OnStartDay += StartDay;
    }

    private void OnDisable()
    {
        Actions.OnStartDay -= StartDay;
    }

    // All input.getkeys are testing inputs. They will be removed later.
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartDay();
        }


        if (_dayTimer.UpdateTimer())
        {
            Debug.Log("Day is over");
            _startCustomer = false;
        }
        else
        {
            float remainingTime = _dayTimer.GetRemainingTime();
            // Convert remaining time into minutes and seconds
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);

            dayTimerText.text = $"{minutes:00}:{seconds:00}";
        }

        if (_startCustomer)
        {
            if (_activeOrders.Count < maxOrders)
            {
                if (_newCustomerTimer.UpdateTimer())
                {
                    GenerateOrder();
                    _newCustomerTimer.ResetTimer();
                }
            }
        }
    }

    private void StartDay()
    {
        // starts the "work day" timer
        _dayTimer.StartTimer();

        // starts the customer timer
        _startCustomer = true;
        GenerateOrder();
        _newCustomerTimer.StartTimer();
    }

    // Generate a random order for a customer
    private void GenerateOrder()
    {
        // If there are no recipes, do nothing
        if (_availableRecipes.Length == 0) return;

        GameObject customerObject = Instantiate(customerPrefab[Random.Range(0, customerPrefab.Length)]);
        Customer customer = customerObject.GetComponent<Customer>();
        RecipeSO assignedOrder;

        if (customer.customerName == "EvilMage")
        {
            assignedOrder = _availableRecipes[0];
        }
        else
        {
            int randomIndex = Random.Range(0, _availableRecipes.Length);
            assignedOrder = _availableRecipes[randomIndex];
        }

        orderManagerUi.GenerateOrderUI(assignedOrder);

        // Add the order to the active orders list
        _activeOrders.Add(new CustomerOrder
        {
            Customer = customer,
            OrderUi = orderManagerUi.GetOrderUI(),
            Recipe = assignedOrder
        });
    }


    internal void FinishOrder(PotionOutput recipe)
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
                Destroy(order.Customer.gameObject);
                order.Customer.OrderComplete(recipe);
                orderManagerUi.RemoveOrderUI(order.OrderUi);

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