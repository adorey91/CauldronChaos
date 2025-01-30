using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QueueManager : MonoBehaviour
{
    [Header("Customer Queue Variables")]
    [SerializeField] private List<GameObject> customerPrefabs; // List of possible customer prefabs
    [SerializeField] private int maxCustomers = 5;
    [SerializeField] private int newCustomerTime = 6;
    private CustomTimer newCustomerTimer;
    private List<GameObject> customers = new();
    private bool startCustomers = false;

    [Header("Customer Queue Positions")]
    [SerializeField] private Transform firstPos;
    [SerializeField] private Transform entryPoint; // Spawn point for new customers
    [SerializeField] private Transform exitPoint; // Spawn point for new customers
    private Vector3[] queuePositions = new Vector3[5]; //queue positions

    [Header("Order Manager")]
    [SerializeField] private OrderManager orderManager;

    [Header("Order Holder")]
    [SerializeField] private GameObject orderHolder;


    public static Action <RecipeSO> OnCheckCustomers;

    public void Start()
    {
        orderHolder = UiManager.uiHolder;

        if(orderHolder == null)
        {
            orderHolder = GameObject.Find("OrderUI_Holder");
        }


        newCustomerTimer = new CustomTimer(newCustomerTime, true);

        queuePositions[0] = firstPos.position;
        for (int i = 1; i < 5; i++)
        {
            queuePositions[i] = firstPos.position + new Vector3(i, 0, 0);
        }
    }

    private void OnEnable()
    {
        Actions.OnEndDay += RemoveAllCustomers;
        Actions.OnStartDay += StartCustomers;
        OnCheckCustomers += CheckCustomerRecipes;
    }

    private void OnDisable()
    {
        Actions.OnEndDay -= RemoveAllCustomers;
        OnCheckCustomers -= CheckCustomerRecipes;
    }


    // using for testing only
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            Actions.OnStartDay?.Invoke();

        if(startCustomers == true)
        {
            if(newCustomerTimer.UpdateTimer())
            {
                SpawnNewCustomer();
                newCustomerTimer.ResetTimer();
            }
        }
    }

    private void StartCustomers()
    {
        startCustomers = true;
        SpawnNewCustomer();
    }

    private void CheckCustomerRecipes(RecipeSO sO)
    {
        for(int i = 0; i < customers.Count; i++)
        {
            if (customers[i].GetComponent<CustomerBehaviour>().requestedOrder == sO)
            {
                customers[i].GetComponent<CustomerBehaviour>().OrderComplete();
                RemoveCustomer(customers[i]);
                return;
            }
        }
        Debug.Log("No customers with that recipe");
    }


    #region Customer Queue Methods
    // Add a new customer to the end of the queue
    public void AddCustomer(GameObject customer)
    {
        customers.Add(customer);
        UpdateQueuePositions();
    }

    // Remove a customer and spawn a new one
    public void RemoveCustomer(GameObject customer)
    {
        if (customers.Contains(customer) && customer.GetComponent<CustomerBehaviour>().hasJoinedQueue)
        {
            customers.Remove(customer);
            customer.GetComponent<CustomerBehaviour>().LeaveQueue(exitPoint.position, () =>
            {
                Destroy(customer);
                SpawnNewCustomer();
            });
            UpdateQueuePositions();
        }
    }

    // Spawn a new customer at the entry point and add them to the queue
    private void SpawnNewCustomer()
    {
        if (customerPrefabs.Count > 0 && customers.Count < maxCustomers)
        {
            int randomIndex = UnityEngine.Random.Range(0, customerPrefabs.Count);
            GameObject newCustomer = Instantiate(customerPrefabs[randomIndex], entryPoint.position, Quaternion.identity);

            CustomerBehaviour _newCustomBehav = newCustomer.GetComponent<CustomerBehaviour>();

            _newCustomBehav.AssignOrder(orderManager.GiveOrder(_newCustomBehav.customerName), orderHolder.transform);

            AddCustomer(newCustomer);
        }
    }

    // Update customer positions in the queue
    private void UpdateQueuePositions()
    {
        for (int i = 0; i < customers.Count; i++)
        {
            customers[i].GetComponent<CustomerBehaviour>().SetTarget(queuePositions[i]);
        }
    }
    #endregion

    private void RemoveAllCustomers()
    {
        foreach (GameObject customer in customers)
        {
            Destroy(customer);
        }
      
        
        customers.Clear();
    }
}
