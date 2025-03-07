using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CauldronMovement : MonoBehaviour
{
    // Agent Variables
    [Header("Movement Settings")]
    private NavMeshAgent _agent;
    private float _movementTime;
    private Vector3 _currentDestination;
    private bool _isMoving = false;
    private CustomTimer _movementTimer;
    [SerializeField] private float minDistance;

    // Model Transform for movement
    [SerializeField] private Transform cauldronModel;

    [SerializeField] private float wanderRadius = 5f;
    [SerializeField] private float minMovementTime = 5f;
    [SerializeField] private float maxMovementTime = 20f;
    [SerializeField] private float liftAmount = 0.1f;
    private Vector3 _startingPos;
    private Coroutine _movement;
    private GameObject _otherCauldron;

    private void Start()
    {
        // Gets main starting position and finds agent
        _startingPos = cauldronModel.transform.localPosition;
        _agent = GetComponent<NavMeshAgent>();
        
        if(_agent != null)
            _agent.avoidancePriority = 30;

        // Finds the other cauldron
        foreach (var cauldron in FindObjectsOfType<CauldronMovement>())
        {
            if (cauldron != this)
                _otherCauldron = cauldron.gameObject;
        }
    }

    // These are used to call the functions for turning the cauldrons on and off.
    #region OnEnable / OnDisable / OnDestroy Events
    private void OnEnable()
    {
        Actions.OnStartCauldron += StartCauldronMovement;
        Actions.OnEndCauldron += () => _isMoving = false;
    }

    private void OnDisable()
    {
        Actions.OnStartCauldron -= StartCauldronMovement;
        Actions.OnEndCauldron -= () => _isMoving = false;
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        Actions.OnStartCauldron -= StartCauldronMovement;
        Actions.OnEndCauldron -= () => _isMoving = false;
        StopAllCoroutines();
    }

    #endregion

    private void FixedUpdate()
    {
        if (!_isMoving) return;
        
        if (_movementTimer.UpdateTimer())
        {
            if (_movement != null)
                StopCoroutine(_movement);
            _movement = StartCoroutine(MoveCauldron());
        }

        // avoidance check
        if (!IsFarFromOtherCauldrons())
        {
            PickNewDestination();
        }
    }

    // Start the movement of the cauldron. Picks a random time between two values then starts a timer.
    private void StartCauldronMovement()
    {
        _isMoving = true;
        _movementTime = Random.Range(minMovementTime, maxMovementTime);
        _movementTimer = new CustomTimer(_movementTime, false);
        _movementTimer.StartTimer();
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
        _movementTime = Random.Range(minMovementTime, maxMovementTime);
        _movementTimer = new CustomTimer(_movementTime, false);
        _movementTimer.StartTimer();
    }

    /// <summary>
    /// Pick a new destination for the cauldron to move.
    /// </summary>
    private void PickNewDestination()
    {
        _currentDestination = RandomNavSphere(transform.position, wanderRadius);
        _agent.SetDestination(_currentDestination);
    }

    // Check if the cauldron is far from other cauldrons
    private bool IsFarFromOtherCauldrons()
    {
        return !(Vector3.Distance(_otherCauldron.transform.position, transform.position) < minDistance);
    }




    // Get a random position on the navmesh
    private Vector3 RandomNavSphere(Vector3 origin, float distance)
    {
        var randomDirection = Random.insideUnitSphere * distance;
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
        return !_agent.pathPending &&
              _agent.remainingDistance <= _agent.stoppingDistance &&
              (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f);
    }
}
