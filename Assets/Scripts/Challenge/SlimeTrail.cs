using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeTrail : MonoBehaviour
{
    [SerializeField] private GameObject slimePrefab;
    [SerializeField] private float distanceBetweenPoints = 1f;
    [SerializeField] private int maxTrailDistance = 10;

    private Vector3 lastSpawnedPosition;
    private Queue<GameObject> slimeTrail = new();
    private bool trailActive = false;

    #region OnEnable / OnDisable / OnDestroy Events
    private void OnEnable()
    {
        Actions.OnStartSlime += StartSlimeTrail;
        Actions.OnEndSlime += EndSlimeTrail;
        Actions.OnResetValues += ResetTrail;
    }

    private void OnDisable()
    {
        Actions.OnStartSlime -= StartSlimeTrail;
        Actions.OnEndSlime -= EndSlimeTrail;
        Actions.OnResetValues -= ResetTrail;
    }

    private void OnDestroy()
    {
        Actions.OnStartSlime -= StartSlimeTrail;
        Actions.OnEndSlime -= EndSlimeTrail;
    }
    #endregion

    private void Update()
    {
        if(trailActive)
        {
            if(Vector3.Distance(lastSpawnedPosition, transform.position) >= distanceBetweenPoints)
            {
                GameObject slime = Instantiate(slimePrefab, transform.position, Quaternion.identity);
                slimeTrail.Enqueue(slime);
                lastSpawnedPosition = transform.position;
            }

            // Remove the oldest slime if it’s too far from the current player position.
            if (slimeTrail.Count > maxTrailDistance)
            {
                    Destroy(slimeTrail.Dequeue());
            }
        }
    }

    private void StartSlimeTrail()
    {
        trailActive = true;
    }

    private void EndSlimeTrail()
    {
        trailActive = false;
    }

    private void ResetTrail()
    {
        foreach (GameObject slime in slimeTrail)
        {
            Destroy(slime);
        }
        slimeTrail.Clear();
    }
}
