using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class CauldronMovement : MonoBehaviour
{
    private NavMeshAgent agent;

    [SerializeField] private bool isMoving = false;
    [SerializeField] private CustomTimer movementTimer;
    private float movementTime;
    private Vector3 currentDestination;
    [SerializeField] private float wanderRadius = 5f;
    [SerializeField] private float minMovementTime = 5f;
    [SerializeField] private float maxMovementTime = 20f;


    public static Action OnStartChallenge;
    public static Action OnEndChallenge;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (isMoving)
        {
            if (movementTimer.UpdateTimer())
            {
                StartCoroutine(MoveCauldron());
            }
        }
    }

    private void OnEnable()
    {
        OnStartChallenge += StartCauldronMovement;
        OnEndChallenge += () => isMoving = false;
    }

    private void OnDisable()
    {
        OnStartChallenge -= StartCauldronMovement;
        OnEndChallenge -= () => isMoving = false;
        StopAllCoroutines();
    }

    private void StartCauldronMovement()
    {
        isMoving = true;
        movementTime = UnityEngine.Random.Range(minMovementTime, maxMovementTime);
        movementTimer = new CustomTimer(movementTime, false);
        movementTimer.StartTimer();
    }

    private IEnumerator MoveCauldron()
    {
        PickNewDestination();

        while (!ReachedTarget())
        {
            yield return null;
        }

        movementTime = UnityEngine.Random.Range(minMovementTime, maxMovementTime);
        movementTimer = new CustomTimer(movementTime, false);
        movementTimer.StartTimer();
    }
    private void PickNewDestination()
    {
        Vector3 newDestination;
        bool validPosition = false;

        int maxAttempts = 10;
        int attempts = 0;

        do
        {
            newDestination = RandomNavSphere(transform.position, wanderRadius);
            validPosition = IsFarFromOtherCauldrons(newDestination);
            attempts++;
        } while (!validPosition && attempts < maxAttempts);

        currentDestination = newDestination;
        agent.SetDestination(currentDestination);
    }

    private bool IsFarFromOtherCauldrons(Vector3 position)
    {
        float minDistance = 2f; // Adjust as needed

        foreach (var cauldron in FindObjectsOfType<CauldronMovement>())
        {
            if (cauldron != this && Vector3.Distance(cauldron.transform.position, position) < minDistance)
            {
                return false;
            }
        }
        return true;
    }



    private Vector3 RandomNavSphere(Vector3 origin, float distance)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;
        randomDirection += origin;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(randomDirection, out navHit, distance, -1))
        {
            return navHit.position;
        }
        return origin; // If no valid point found, stay put
    }



    private bool ReachedTarget()
    {
        return !agent.pathPending &&
              agent.remainingDistance <= agent.stoppingDistance &&
              (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
    }
}
