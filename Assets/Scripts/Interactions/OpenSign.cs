using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OpenSign : MonoBehaviour, IInteractable
{ 
    [SerializeField] private Material openMaterial;
    [SerializeField] private TextMeshProUGUI signText;

    [Header("Dropped Item Holder")]
    [SerializeField] private GameObject droppedItems;

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
        CleanUpDroppedItems();
        _isOpen = true;
        Actions.OnStartDay?.Invoke();
        signText.text = "Open";
        GetComponent<MeshRenderer>().material = openMaterial;
    }

    private void CloseSignBoard()
    {
        //Actions.OnEndDay?.Invoke();
        _isOpen = false;
        CleanUpDroppedItems();
        signText.text = "Closed";
        GetComponent<MeshRenderer>().material = openMaterial;
    }

    private bool CleanUpDroppedItems()
    {
        if (droppedItems.transform.childCount > 0)
        {
            foreach (Transform child in droppedItems.transform)
            {
                Destroy(child.gameObject);
            }
            return true;
        }
        return false;
    }
}