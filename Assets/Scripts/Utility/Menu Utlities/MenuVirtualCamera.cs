using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MenuVirtualCamera : MonoBehaviour
{
    [SerializeField] private Cinemachine.CinemachineVirtualCamera cinemachineCamera;
    [SerializeField] private Camera cameraBrain;
    [SerializeField] Transform doorTarget;
    [SerializeField] Transform calenderTarget;
    [SerializeField] Transform cauldronTarget;

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
        cinemachineCamera.m_LookAt = doorTarget;
        Cinemachine.CinemachineTrackedDolly dolly = cinemachineCamera.GetCinemachineComponent<Cinemachine.CinemachineTrackedDolly>();
        dolly.m_PathPosition = 0;
    }

    private void LookAtCalendar()
    {
        cinemachineCamera.m_LookAt = calenderTarget;
        var dolly = cinemachineCamera.GetCinemachineComponent<CinemachineTrackedDolly>();

        dolly.m_PathPosition = 2; // Move cameraBrain
    }

    private void ResetCamera()
    {
        cameraBrain.enabled = false;
        cinemachineCamera.m_LookAt = cauldronTarget;
        var dolly = cinemachineCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
        dolly.m_PathPosition = 1;
    }
}
