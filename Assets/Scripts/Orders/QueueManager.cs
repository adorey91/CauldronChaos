using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
using DG.Tweening;

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


    public static Action<PotionOutput> OnCheckCustomers;

    public void Start()
    {
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
        Actions.OnResetValues += RemoveAllCustomers;
    }

    private void OnDisable()
    {
        Actions.OnEndDay -= RemoveAllCustomers;
        Actions.OnStartDay -= StartCustomers;
        OnCheckCustomers -= CheckCustomerRecipes;
        Actions.OnResetValues -= RemoveAllCustomers;
    }


    // using for testing only
    private void Update()
    {
        if (startCustomers == true)
        {
            if (newCustomerTimer.UpdateTimer())
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

    private void CheckCustomerRecipes(PotionOutput potionOutput)
    {
        RecipeSO sO = potionOutput.potionInside;
        GameObject potionObj = potionOutput.gameObject;

        for (int i = 0; i < customers.Count; i++)
        {
            if (customers[i].GetComponent<CustomerBehaviour>().requestedOrder == sO)
            {
                potionOutput.givenToCustomer = true;
                potionObj.GetComponent<Rigidbody>().isKinematic = true;

                CustomerBehaviour servingCustomer = customers[i].GetComponent<CustomerBehaviour>();

                potionObj.transform.SetParent(servingCustomer.customerHands);
                potionObj.transform.DOJump(servingCustomer.customerHands.position, 1, 1, 0.3f).OnComplete(() => FinishOrder(servingCustomer, potionObj));
                return;
            }
        }

        Debug.Log("No customers with that recipe");
        potionObj.transform.DOScale(Vector3.zero, 1).OnComplete(() => Destroy(potionObj));
    }

    private void FinishOrder(CustomerBehaviour servingCustomer, GameObject obj)
    {
        servingCustomer.OrderComplete();
        RemoveCustomer(servingCustomer.gameObject);
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
        startCustomers = false;
    }
}
