using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class QueueManager : MonoBehaviour
{
    [Header("Customer Queue Variables")]
    [SerializeField] private int timeForCustomer = 1;
    [SerializeField] private List<GameObject> customerPrefabs; // List of possible customer prefabs
    private List<GameObject> _customers = new();
    private int _maxCustomers = 5;
    private CustomTimer _newCustomer;
    private bool _startCustomers;
    private bool _newDay;
    private int _previousIndex;

    [Header("Customer Queue Positions")]
    [SerializeField] private Transform firstPos;
    [SerializeField] private Transform entryPoint; // Spawn point for new customers
    [SerializeField] private Transform exitPoint; // Spawn point for new customers
    private Vector3[] _queuePositions = new Vector3[5]; //queue positions

    [Header("Order Manager")]
    [SerializeField] private OrderManager orderManager;

    [Header("Order Holder")]
    [SerializeField] private GameObject orderHolder;

    [Header("SFX")]
    [SerializeField] private AudioClip potionSaleSfx;


    public void Start()
    {
        _newCustomer = new CustomTimer(timeForCustomer, true);

        _queuePositions[0] = firstPos.position;
        for (int i = 1; i < 5; i++)
        {
            _queuePositions[i] = firstPos.position + new Vector3(i, 0, 0);
        }
    }

    private void Update()
    {
        // For testing purposes
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            StartCustomers();
        }

        if (_startCustomers != true) return;
        if (_newCustomer.UpdateTimer())
        {
            SpawnNewCustomer();
            _newCustomer.ResetTimer();
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
        _startCustomers = true;
        _newDay = true;
        _newCustomer = new CustomTimer(3, false);
        _newCustomer.StartTimer();
        SpawnNewCustomer();
    }

    private void CheckCustomerRecipes(PotionOutput potionOutput)
    {
        var sO = potionOutput.potionInside;
        var potionObj = potionOutput.gameObject;

        for (var i = 0; i < _customers.Count; i++)
        {
            var customer = _customers[i].GetComponent<CustomerBehaviour>();

            if (customer.RequestedOrder == sO && customer.HasJoinedQueue)
            {
                potionOutput.givenToCustomer = true;
                potionObj.GetComponent<Collider>().enabled = false;
                potionObj.GetComponent<Rigidbody>().isKinematic = true;

                CustomerBehaviour servingCustomer = _customers[i].GetComponent<CustomerBehaviour>();

                potionObj.transform.SetParent(servingCustomer.customerHands);
                potionObj.transform.DOJump(servingCustomer.customerHands.position, 1, 1, 0.3f).OnComplete(() => FinishOrder(servingCustomer));
                return;
            }
        }

        var startPos = potionObj.transform.position;
        var randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(0.5f, 2f));
        var endPos = startPos + randomDirection * 3f;
        potionObj.transform.DOJump(endPos, 2, 1, 1);
    }

    private void FinishOrder(CustomerBehaviour servingCustomer)
    {
        servingCustomer.OrderComplete();

        //playing SFX for potion sale
        AudioManager.instance.sfxManager.PlaySFX(SFX_Type.ShopSounds, potionSaleSfx, true);

        RemoveCustomer(servingCustomer.gameObject);
    }

    #region Customer Queue Methods
    // Add a new customer to the end of the queue
    private void AddCustomer(GameObject customer)
    {
        _customers.Add(customer);
        UpdateQueuePositions();
    }

    // Remove a customer and spawn a new one
    private void RemoveCustomer(GameObject customer)
    {
        if (_customers.Contains(customer) && customer.GetComponent<CustomerBehaviour>().HasJoinedQueue)
        {
            _customers.Remove(customer);
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
        if (customerPrefabs.Count > 0 && _customers.Count < _maxCustomers)
        {
            int randomIndex = Random.Range(0, customerPrefabs.Count);

            while(randomIndex == _previousIndex)
            {
                randomIndex = Random.Range(0, customerPrefabs.Count);
            }

            _previousIndex = randomIndex;
            var newCustomer = Instantiate(customerPrefabs[randomIndex], entryPoint.position, Quaternion.identity);

            var newCustomBehaviour = newCustomer.GetComponent<CustomerBehaviour>();

            newCustomBehaviour.AssignOrder(orderManager.GiveOrder(newCustomBehaviour.customerName), orderHolder.transform);

            AddCustomer(newCustomer);
        }
        else
        {
            if (_newDay)
            {
                _newCustomer = new CustomTimer(timeForCustomer, false);
                _newDay = false;
            }

        }
    }

    // Update customer positions in the queue
    private void UpdateQueuePositions()
    {
        for (int i = 0; i < _customers.Count; i++)
        {
            _customers[i].GetComponent<CustomerBehaviour>().SetTarget(_queuePositions[i]);
        }
    }
    #endregion

    internal void ScareCustomer()
    {
        List<GameObject> customersInQueue = new();

        foreach (var customer in _customers)
        {
            if (customer.GetComponent<CustomerBehaviour>().HasJoinedQueue)
            {
                customersInQueue.Add(customer);
            }
        }

        if (customersInQueue.Count > 0)
        {
            var random = Random.Range(0, customersInQueue.Count);
            var scaredCustomer = customersInQueue[random].GetComponent<CustomerBehaviour>();
            scaredCustomer.ScareAway();
            RemoveCustomer(_customers[random]);
        }
    }

    private void RemoveAllCustomers()
    {
        _startCustomers = false;

        foreach (GameObject customer in _customers)
        {
            Destroy(customer);
        }

        _newDay = false;
        _customers.Clear();
    }


    internal int AreThereCustomers()
    {
        var count = 0;

        foreach (var customer in _customers)
        {
            if (customer.GetComponent<CustomerBehaviour>().HasJoinedQueue)
                count++;
        }

        return count;
    }
}
