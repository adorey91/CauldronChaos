using Cinemachine;
using UnityEngine;
using System;

public class MenuVirtualCamera : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cineCamera;
    [SerializeField] private Camera cameraBrain;
    [SerializeField] private Transform doorTarget;
    [SerializeField] private Transform calenderTarget;
    [SerializeField] private Transform cauldronTarget;

    public static Action OnResetCamera;
    public static Action TurnCameraBrainOn;


    #region OnEnable / OnDisable / OnDestroy Events
    private void OnEnable()
    {
        Actions.OnSaveExist += MoveMainMenuCamera;
        OnResetCamera += ResetCamera;
        TurnCameraBrainOn += TurnOnCameraBrain;
    }

    private void OnDisable()
    {
        Actions.OnSaveExist -= MoveMainMenuCamera;
        OnResetCamera -= ResetCamera;
        TurnCameraBrainOn -= TurnOnCameraBrain;
    }

    private void OnDestroy()
    {
        Actions.OnSaveExist -= MoveMainMenuCamera;
        OnResetCamera -= ResetCamera;
        TurnCameraBrainOn -= TurnOnCameraBrain;
    }
    #endregion

    private void TurnOnCameraBrain()
    {
        cameraBrain.enabled = true;
    }

    private void MoveMainMenuCamera(bool saveExists)
    {
        if (saveExists)
            LookAtCalendar();
        else
            Intro();
    }

    private void Intro()
    {
        cineCamera.m_LookAt = doorTarget;
        var dolly = cineCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
        dolly.m_PathPosition = 0;
    }

    private void LookAtCalendar()
    {
        cineCamera.m_LookAt = calenderTarget;
        var dolly = cineCamera.GetCinemachineComponent<CinemachineTrackedDolly>();

        dolly.m_PathPosition = 2; // Move cameraBrain
    }

    private void ResetCamera()
    {
        cameraBrain.enabled = false;
        cineCamera.m_LookAt = cauldronTarget;
        var dolly = cineCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
        dolly.m_PathPosition = 1;
    }
}
