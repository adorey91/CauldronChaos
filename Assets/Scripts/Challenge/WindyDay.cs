using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindyDay : MonoBehaviour
{
    public enum WindDirection
    {
        GoingLeft,
        GoingRight,
        TowardsScreen,
        AwayFromScreen
    }
    public WindDirection windDirect;

    public float strength = 5;
    private float defaultWindStrength;
    public Vector3 direction;
    private CustomTimer windDirectionChange;
    private readonly float windChangeTime = 30f;
    [SerializeField] private GameObject[] windows;


    private void Awake()
    {
        defaultWindStrength = strength;
        windDirectionChange = new(windChangeTime, false);
    }

    #region OnEnable / OnDisable / OnDestroy Events
    private void OnEnable()
    {
        Actions.OnStartWindy += StartWind;
        Actions.OnStopWindy += StopWind;
    }

    private void OnDisable()
    {
        Actions.OnStartWindy -= StartWind;
        Actions.OnStopWindy -= StopWind;
    }

    private void OnDestroy()
    {
        Actions.OnStartWindy -= StartWind;
        Actions.OnStopWindy -= StopWind;
    }
    #endregion

    private void Update()
    {
        if(windDirectionChange.UpdateTimer())
        {
            ChangeWindDirection();
        }
    }

    internal void StartWind()
    {
        foreach(var window in windows)
        {
            window.SetActive(false);
        }
        strength = defaultWindStrength;
        ChangeWindDirection();
    }

    private void StopWind()
    {
        strength = 0;
        foreach (var window in windows)
        {
            window.SetActive(true);
        }
    }

    private void ChangeWindDirection()
    {
        windDirect = (WindDirection)Random.Range(0, 4);
        
        switch (windDirect)
        {
            case WindDirection.GoingLeft: direction = -transform.right; break;
            case WindDirection.GoingRight: direction = transform.right; break;
            case WindDirection.TowardsScreen: direction = -transform.forward; break;
            case WindDirection.AwayFromScreen: direction = transform.forward; break;
        }
        Debug.Log("Wind Direction Changed to: " + windDirect);

        windDirectionChange.ResetTimer();
    }
}
