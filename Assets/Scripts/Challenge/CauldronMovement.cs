using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CauldronMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private float movementTime;
    private Vector3 currentDestination;

    [SerializeField] private bool isMoving = false;
    [SerializeField] private float wanderRadius = 5f;
    [SerializeField] private float minMovementTime = 5f;
    [SerializeField] private float maxMovementTime = 20f;
    [SerializeField] private CustomTimer movementTimer;

    // Events
    /// <summary> Event to start the challenge </summary>
    public static System.Action OnStartChallenge;
    /// <summary> Event to end the challenge </summary>
    public static System.Action OnEndChallenge;

    private Coroutine movement;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
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

    private void FixedUpdate()
    {
        if (isMoving)
        {
            if (movementTimer.UpdateTimer())
            {
                StopCoroutine(movement);
                movement = StartCoroutine(MoveCauldron());
            }
        }
    }

    // Start the movement of the cauldron
    private void StartCauldronMovement()
    {
        isMoving = true;
        movementTime = Random.Range(minMovementTime, maxMovementTime);
        movementTimer = new CustomTimer(movementTime, false);
        movementTimer.StartTimer();
    }

    // Coroutine to move the cauldron
    private IEnumerator MoveCauldron()
    {
        PickNewDestination();

        // Wait until the cauldron reaches the destination
        while (!ReachedTarget())
        {
            yield return null;
        }

        // Start the timer again
        movementTime = Random.Range(minMovementTime, maxMovementTime);
        movementTimer = new CustomTimer(movementTime, false);
        movementTimer.StartTimer();
    }

    // Pick a new destination for the cauldron
    private void PickNewDestination()
    {
        Vector3 newDestination;
        bool validPosition = false;

        int maxAttempts = 10;
        int attempts = 0;

        // Try to find a valid position for the cauldron
        do
        {
            newDestination = RandomNavSphere(transform.position, wanderRadius);
            validPosition = IsFarFromOtherCauldrons(newDestination);
            attempts++;
        } while (!validPosition && attempts < maxAttempts);

        currentDestination = newDestination;
        agent.SetDestination(currentDestination);
    }

    // Check if the cauldron is far from other cauldrons
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


    // Get a random position on the navmesh
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


    // Check if the cauldron has reached the target
    private bool ReachedTarget()
    {
        return !agent.pathPending &&
              agent.remainingDistance <= agent.stoppingDistance &&
              (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
    }
}
