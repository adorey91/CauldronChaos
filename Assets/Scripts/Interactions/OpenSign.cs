using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OpenSign : MonoBehaviour, IInteractable
{
    [SerializeField] private Material openMaterial;
    [SerializeField] private TextMeshProUGUI signText;
    private bool _isOpen;

    public void Start()
    {
        _isOpen = false;
        signText.text = "Closed";
    }

    private void OnEnable()
    {
        Actions.OnEndDay += CloseSignBoard;
    }

    private void OnDisable()
    {
        Actions.OnEndDay -= CloseSignBoard;
    }

    public void Interact(InteractionDetector player)
    {
        if(!_isOpen)
            OpenSignBoard();
    }

    private void OpenSignBoard()
    {
        _isOpen = true;
        Actions.OnStartDay?.Invoke();
        signText.text = "Open";
        GetComponent<MeshRenderer>().material = openMaterial;
    }

    private void CloseSignBoard()
    {
        //Actions.OnEndDay?.Invoke();
        _isOpen = false;
        signText.text = "Closed";
        GetComponent<MeshRenderer>().material = openMaterial;
    }
}