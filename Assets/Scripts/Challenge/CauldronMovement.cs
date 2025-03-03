using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CauldronMovement : MonoBehaviour
{
    // Agent Variables
    private NavMeshAgent agent;
    private float movementTime;
    private Vector3 currentDestination;
    private bool isMoving = false;
    private CustomTimer movementTimer;

    // Model Transform for movement
    [SerializeField] private Transform cauldronModel;

    [SerializeField] private float wanderRadius = 5f;
    [SerializeField] private float minMovementTime = 5f;
    [SerializeField] private float maxMovementTime = 20f;
    [SerializeField] private float liftAmount = 0.1f;
    private Vector3 _startingPos;
    private Coroutine movement;
    private GameObject otherCauldron;

    private bool isFarFromCauldron;

    private void Start()
    {
        // Gets main starting position and finds agent
        _startingPos = cauldronModel.transform.localPosition;
        agent = GetComponent<NavMeshAgent>();
        agent.avoidancePriority = 30;

        foreach (var cauldron in FindObjectsOfType<CauldronMovement>())
        {
            if (cauldron != this)
                otherCauldron = cauldron.gameObject;
        }
    }

    // These are used to call the functions for turning the cauldrons on and off.
    #region OnEnable / OnDisable / OnDestroy Events
    private void OnEnable()
    {
        Actions.OnStartCauldron += StartCauldronMovement;
        Actions.OnEndCauldron += () => isMoving = false;
    }

    private void OnDisable()
    {
        Actions.OnStartCauldron -= StartCauldronMovement;
        Actions.OnEndCauldron -= () => isMoving = false;
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        Actions.OnStartCauldron -= StartCauldronMovement;
        Actions.OnEndCauldron -= () => isMoving = false;
        StopAllCoroutines();
    }

    #endregion

    private void FixedUpdate()
    {
        if (isMoving)
        {
            if (movementTimer.UpdateTimer())
            {
                if (movement != null)
                    StopCoroutine(movement);
                movement = StartCoroutine(MoveCauldron());
            }

            // avoidance check
            if (!IsFarFromOtherCauldrons())
            {
                PickNewDestination();
            }
        }
    }

    // Start the movement of the cauldron. Picks a random time between two values then starts a timer.
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
        cauldronModel.DOLocalMoveY(liftAmount, 0.5f);
        // Wait until the cauldron reaches the destination
        while (!ReachedTarget())
        {
            yield return null;
        }

        // Start the timer again
        cauldronModel.DOLocalMove(_startingPos, 0.5f);
        movementTime = Random.Range(minMovementTime, maxMovementTime);
        movementTimer = new CustomTimer(movementTime, false);
        movementTimer.StartTimer();
    }

    /// <summary>
    /// Pick a new destination for the cauldron to move.
    /// </summary>
    private void PickNewDestination()
    {
        currentDestination = RandomNavSphere(transform.position, wanderRadius);
        agent.SetDestination(currentDestination);
    }

    // Check if the cauldron is far from other cauldrons
    private bool IsFarFromOtherCauldrons()
    {
        float minDistance = 1.5f; // Adjust as needed

        if (Vector3.Distance(otherCauldron.transform.position, transform.position) < minDistance)
        {
            isFarFromCauldron = false;
            return false;
        }
        isFarFromCauldron = true;
        return true;
    }




    // Get a random position on the navmesh
    private Vector3 RandomNavSphere(Vector3 origin, float distance)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, distance, -1))
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
