using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour
{
    [Header("Order Variables")]
    [SerializeField] private RecipeManager recipeManager;
    [SerializeField] private OrderCounter orderCounter;
    [SerializeField] private int maxOrders = 4;
    private RecipeSO[] _availableRecipes;

    [Header("Order UI")]
    [SerializeField] private OrderManagerUi orderManagerUi;

    [Header("Customer Prefab")]
    [SerializeField] GameObject[] customerPrefab;
    [SerializeField] private Transform customerSpawnPoint;

    [Header("Active Customers")]
    private List<CustomerOrder> _activeOrders = new();

    // Timer for the day - sets 5 minute timer
    [Header("Day Timer Settings")]
    [SerializeField] private int minutesPerDay = 5;
    [SerializeField] private TextMeshProUGUI dayTimerText;
    private CustomTimer _dayTimer;
    [SerializeField] private RectTransform timerHandRect;
    bool timerStarted = false;
    float rotationSpeed;
    float secondsToDegrees = 180f / 60f;

    private Quaternion _minAngle = Quaternion.Euler(0, 0, 90);
    private Quaternion _maxAngle = Quaternion.Euler(0, 0, -90);

    private bool _startCustomer = false;

    // Customer Order Timer - this time is in seconds
    [Header("Customer Timer Settings")]
    [SerializeField] private int newCustomerTime = 6;
    private CustomTimer _newCustomerTimer;


    private void Start()
    {
        recipeManager = FindObjectOfType<RecipeManager>();
        orderCounter = FindObjectOfType<OrderCounter>();
        _dayTimer = new CustomTimer(minutesPerDay, true);
        _newCustomerTimer = new CustomTimer(newCustomerTime, false);
        _availableRecipes = recipeManager.FindAvailableRecipes();
        _activeOrders.Clear();

        timerHandRect.localRotation = _minAngle;
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
        if (_dayTimer.UpdateTimer())
        {
            Debug.Log("Day is over");
            Actions.OnEndDay?.Invoke();
            Actions.OnForceStateChange("EndOfDay");
            timerStarted = false;
            _startCustomer = false;
            EndDay();
        }

        if (timerStarted)
        {
            float remainingTime = _dayTimer.GetRemainingTime();

            // Convert remaining time into minutes and seconds
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);

            dayTimerText.text = $"{minutes:00}:{seconds:00}";

            float rotationAngle = remainingTime * secondsToDegrees;

            // This works but it seems backwards? Rotate towards should be from -> to but this is to -> from. Will need to investigate
            timerHandRect.rotation = Quaternion.RotateTowards(_maxAngle, _minAngle, rotationAngle);

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
        timerStarted = true;
        // starts the customer timer
        _startCustomer = true;
        GenerateOrder();
        _newCustomerTimer.StartTimer();
    }

    private void EndDay()
    {
        timerStarted = false;
        RemoveAllOrders();
        timerHandRect.transform.rotation = _minAngle;
    }

    // Generate a random order for a customer
    private void GenerateOrder()
    {
        //// If there are no recipes, do nothing
        //if (_availableRecipes.Length == 0) return;

        //GameObject customerObject = Instantiate(customerPrefab[Random.Range(0, customerPrefab.Length)], orderCounter.GetNextPosition(), transform.rotation * Quaternion.Euler(0, 180f, 0), orderCounter.ParentPosition());
        //Customer customer = customerObject.GetComponent<Customer>();
        //RecipeSO assignedOrder;

        //if (customer.customerName == "EvilMage")
        //{
        //    assignedOrder = _availableRecipes[0];
        //}
        //else
        //{
        //    int randomIndex = Random.Range(0, _availableRecipes.Length);
        //    assignedOrder = _availableRecipes[randomIndex];
        //}

        //orderManagerUi.GenerateOrderUI(assignedOrder);

        //// Add the order to the active orders list
        //_activeOrders.Add(new CustomerOrder
        //{
        //    Customer = customer,
        //    OrderUi = orderManagerUi.GetOrderUI(),
        //    Recipe = assignedOrder
        //});
    }


    internal void FinishOrder(PotionOutput recipe)
    {
        //if (_activeOrders.Count == 0)
        //{
        //    Debug.Log("No customers to serve");
        //    return;
        //}

        //foreach (var order in _activeOrders)
        //{
        //    if (order.Recipe == recipe.potionInside)
        //    {
        //        Vector3 customerPosition = order.Customer.transform.position;

        //        Destroy(order.Customer.gameObject);
        //        order.Customer.OrderComplete(recipe);
        //        orderManagerUi.RemoveOrderUI(order.OrderUi);

        //        _activeOrders.Remove(order);

        //        // Free the customer's position in the queue
        //        orderCounter.FreePosition(customerPosition);

        //        return;
        //    }
        //}

        //Actions.OnNoCustomerServed?.Invoke();
        //Debug.Log("No customer found with that order");
    }


    private void RemoveAllOrders()
    {
        //foreach (var order in _activeOrders)
        //{
        //    Destroy(order.Customer.gameObject);
        //    orderManagerUi.RemoveOrderUI(order.OrderUi);
        //}

        //_activeOrders.Clear();
    }
}