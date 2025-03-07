using System.Collections.Generic;
using UnityEngine;

public class SlimeTrail : MonoBehaviour
{
    [SerializeField] private GameObject slimePrefab;
    [SerializeField] private float distanceBetweenPoints = 1f;
    [SerializeField] private int maxTrailDistance = 10;

    private Vector3 _lastSpawnedPosition;
    private Queue<GameObject> _slimeTrail = new();
    private bool _trailActive = false;

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
        if (!_trailActive) return;
        
        if(Vector3.Distance(_lastSpawnedPosition, transform.position) >= distanceBetweenPoints)
        {
            GameObject slime = Instantiate(slimePrefab, transform.position, Quaternion.identity);
            _slimeTrail.Enqueue(slime);
            _lastSpawnedPosition = transform.position;
        }

        // Remove the oldest slime if it�s too far from the current player position.
        if (_slimeTrail.Count > maxTrailDistance)
        {
            Destroy(_slimeTrail.Dequeue());
        }
    }

    private void StartSlimeTrail()
    {
        _trailActive = true;
    }

    private void EndSlimeTrail()
    {
        _trailActive = false;
    }

    private void ResetTrail()
    {
        foreach (var slime in _slimeTrail)
        {
            Destroy(slime);
        }
        _slimeTrail.Clear();
    }
}
