using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class CauldronMovement : MonoBehaviour
{
    private NavMeshAgent agent;

    [SerializeField] private bool isMoving = false;
    private CustomTimer movementTimer;
    private float movementTime;
    private float minMovementTime = 5f;
    private float maxMovementTime = 20f;

    public static Action OnStartChallenge;

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
    }

    private void OnDisable()
    {
        OnStartChallenge -= StartCauldronMovement;
        StopAllCoroutines();
    }

    private void StartCauldronMovement()
    {
        agent = GetComponent<NavMeshAgent>();
        isMoving = true;
        movementTime = UnityEngine.Random.Range(minMovementTime, maxMovementTime);
        movementTimer = new CustomTimer(movementTime, false);
    }

    private IEnumerator MoveCauldron()
    {
        agent.SetDestination(new Vector3(UnityEngine.Random.Range(-10, 10), 0, UnityEngine.Random.Range(-10, 10)));

        while (!ReachedTarget())
        {
            yield return null;
        }

        movementTime = UnityEngine.Random.Range(minMovementTime, maxMovementTime);
        movementTimer = new CustomTimer(movementTime, false);

    }

    private bool ReachedTarget()
    {
        return !agent.pathPending &&
              agent.remainingDistance <= agent.stoppingDistance &&
              (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
    }
}
