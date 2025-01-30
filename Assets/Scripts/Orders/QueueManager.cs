using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> customers = new List<GameObject>();
    [SerializeField] private Transform firstPos;
    [SerializeField] private Transform entryPoint; // Spawn point for new customers
    [SerializeField] private Transform exitPoint; // Spawn point for new customers
    [SerializeField] private List<GameObject> customerPrefabs; // List of possible customer prefabs
    [SerializeField] private int maxCustomers = 5;
    private Vector3[] queuePositions = new Vector3[5]; //queue positions


    public void Start()
    {
        queuePositions[0] = firstPos.position;
        for (int i = 1; i < 5; i++)
        {
            queuePositions[i] = firstPos.position + new Vector3(i, 0, 0);
        }
    }

    private void OnEnable()
    {
        Actions.OnEndDay += RemoveAllCustomers;
    }

    private void OnDisable()
    {
        Actions.OnEndDay -= RemoveAllCustomers;
    }

    //used for testing
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            SpawnNewCustomer();

        if (Input.GetKeyDown(KeyCode.R))
        {
            int randomIndex = Random.Range(0, customers.Count);
            RemoveCustomer(customers[randomIndex]);
        }
    }


    // Add a new customer to the end of the queue
    public void AddCustomer(GameObject customer)
    {
        customers.Add(customer);
        UpdateQueuePositions();
    }

    // Remove a customer and spawn a new one
    public void RemoveCustomer(GameObject customer)
    {
        if (customers.Contains(customer))
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
            int randomIndex = Random.Range(0, customerPrefabs.Count);
            GameObject newCustomer = Instantiate(customerPrefabs[randomIndex], entryPoint.position, Quaternion.identity);
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

    private void RemoveAllCustomers()
    {
        foreach (GameObject customer in customers)
        {
            Destroy(customer);
        }
        customers.Clear();
    }
}
