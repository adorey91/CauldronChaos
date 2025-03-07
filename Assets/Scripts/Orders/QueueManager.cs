using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class QueueManager : MonoBehaviour
{
    [Header("Customer Queue Variables")]
    [SerializeField] private int timeForCustomer = 1;
    [SerializeField] private List<GameObject> customerPrefabs; // List of possible customer prefabs
    private List<GameObject> customers = new();
    private int maxCustomers = 5;
    private CustomTimer newCustomer;
    private bool startCustomers = false;
    private bool newDay;
    private int previousIndex;

    [Header("Customer Queue Positions")]
    [SerializeField] private Transform firstPos;
    [SerializeField] private Transform entryPoint; // Spawn point for new customers
    [SerializeField] private Transform exitPoint; // Spawn point for new customers
    private Vector3[] queuePositions = new Vector3[5]; //queue positions

    [Header("Order Manager")]
    [SerializeField] private OrderManager orderManager;

    [Header("Order Holder")]
    [SerializeField] private GameObject orderHolder;

    [Header("SFX")]
    [SerializeField] private AudioClip potionSaleSFX;


    public void Start()
    {
        newCustomer = new CustomTimer(timeForCustomer, true);

        queuePositions[0] = firstPos.position;
        for (int i = 1; i < 5; i++)
        {
            queuePositions[i] = firstPos.position + new Vector3(i, 0, 0);
        }
    }

    private void Update()
    {
        // For testing purposes
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            StartCustomers();
        }

        if (startCustomers == true)
        {
            if (newCustomer.UpdateTimer())
            {
                SpawnNewCustomer();
                newCustomer.ResetTimer();
            }
        }
    }

    #region OnEnable / OnDisable / OnDestroy Events
    private void OnEnable()
    {
        Actions.OnEndDay += RemoveAllCustomers;
        Actions.OnStartDay += StartCustomers;
        Actions.OnCheckCustomers += CheckCustomerRecipes;
        Actions.OnResetValues += RemoveAllCustomers;
    }

    private void OnDisable()
    {
        Actions.OnEndDay -= RemoveAllCustomers;
        Actions.OnStartDay -= StartCustomers;
        Actions.OnCheckCustomers -= CheckCustomerRecipes;
        Actions.OnResetValues -= RemoveAllCustomers;
    }

    private void OnDestroy()
    {
        Actions.OnEndDay -= RemoveAllCustomers;
        Actions.OnStartDay -= StartCustomers;
        Actions.OnCheckCustomers -= CheckCustomerRecipes;
        Actions.OnResetValues -= RemoveAllCustomers;
    }
    #endregion

    private void StartCustomers()
    {
        startCustomers = true;
        newDay = true;
        newCustomer = new CustomTimer(3, false);
        newCustomer.StartTimer();
        SpawnNewCustomer();
    }

    private void CheckCustomerRecipes(PotionOutput potionOutput)
    {
        RecipeSO sO = potionOutput.potionInside;
        GameObject potionObj = potionOutput.gameObject;

        for (int i = 0; i < customers.Count; i++)
        {
            CustomerBehaviour customer = customers[i].GetComponent<CustomerBehaviour>();

            if (customer.requestedOrder == sO && customer.hasJoinedQueue)
            {
                potionOutput.givenToCustomer = true;
                potionObj.GetComponent<Collider>().enabled = false;
                potionObj.GetComponent<Rigidbody>().isKinematic = true;

                CustomerBehaviour servingCustomer = customers[i].GetComponent<CustomerBehaviour>();

                potionObj.transform.SetParent(servingCustomer.customerHands);
                potionObj.transform.DOJump(servingCustomer.customerHands.position, 1, 1, 0.3f).OnComplete(() => FinishOrder(servingCustomer));
                return;
            }
        }

        Vector3 startPos = potionObj.transform.position;
        Vector3 randomDirection = new (Random.Range(-1f, 1f), 0, Random.Range(0.5f, 2f));
        Vector3 endPos = startPos + randomDirection * 3f;
        potionObj.transform.DOJump(endPos, 2, 1, 1);
    }

    private void FinishOrder(CustomerBehaviour servingCustomer)
    {
        servingCustomer.OrderComplete();

        //playing SFX for potion sale
        AudioManager.instance.sfxManager.PlaySFX(SFX_Type.ShopSounds, potionSaleSFX, true);

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

            while(randomIndex == previousIndex)
            {
                randomIndex = UnityEngine.Random.Range(0, customerPrefabs.Count);
            }

            previousIndex = randomIndex;
            GameObject newCustomer = Instantiate(customerPrefabs[randomIndex], entryPoint.position, Quaternion.identity);

            CustomerBehaviour _newCustomBehav = newCustomer.GetComponent<CustomerBehaviour>();

            _newCustomBehav.AssignOrder(orderManager.GiveOrder(_newCustomBehav.customerName), orderHolder.transform);

            AddCustomer(newCustomer);
        }
        else
        {
            if (newDay)
            {
                newCustomer = new CustomTimer(timeForCustomer, false);
                newDay = false;
            }

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

    internal void ScareCustomer()
    {
        List<GameObject> customersInQueue = new();

        foreach (GameObject customer in customers)
        {
            if (customer.GetComponent<CustomerBehaviour>().hasJoinedQueue)
            {
                customersInQueue.Add(customer);
            }
        }

        if (customersInQueue.Count > 0)
        {
            int random = Random.Range(0, customersInQueue.Count);
            CustomerBehaviour scaredCustomer = customersInQueue[random].GetComponent<CustomerBehaviour>();
            scaredCustomer.ScareAway();
            RemoveCustomer(customers[random]);
        }
    }

    private void RemoveAllCustomers()
    {
        startCustomers = false;

        foreach (GameObject customer in customers)
        {
            Destroy(customer);
        }

        newDay = false;
        customers.Clear();
    }


    internal int AreThereCustomers()
    {
        int count = 0;

        foreach (GameObject customer in customers)
        {
            if (customer.GetComponent<CustomerBehaviour>().hasJoinedQueue)
                count++;
        }

        return count;
    }
}
