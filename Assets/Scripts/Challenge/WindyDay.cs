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
    private float _defaultWindStrength;
    public Vector3 direction;
    private CustomTimer _windDirectionChange;
    private readonly float _windChangeTime = 30f;
    [SerializeField] private GameObject[] windows;


    private void Awake()
    {
        _defaultWindStrength = strength;
        _windDirectionChange = new CustomTimer(_windChangeTime, false);
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
        if(_windDirectionChange.UpdateTimer())
        {
            ChangeWindDirection();
        }
    }

    private void StartWind()
    {
        foreach(var window in windows)
        {
            window.SetActive(false);
        }
        strength = _defaultWindStrength;
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

        _windDirectionChange.ResetTimer();
    }
}
