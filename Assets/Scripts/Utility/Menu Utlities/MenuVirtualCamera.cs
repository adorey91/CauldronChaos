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

    private void OnEnable()
    {
        SaveLoad.OnSaveExist += MoveMainMenuCamera;
        OnResetCamera += ResetCamera;
    }

    private void OnDisable()
    {
        SaveLoad.OnSaveExist -= MoveMainMenuCamera;
        OnResetCamera -= ResetCamera;
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
