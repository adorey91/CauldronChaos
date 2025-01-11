using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OpenSign : MonoBehaviour, IInteractable
{
    [SerializeField] private Material openMaterial;
    [SerializeField] private TextMeshProUGUI signText;

    public void Start()
    {
        signText.text = "Closed";
    }

    public void Interact(InteractionDetector player)
    {
        Debug.Log("Interacting with Open Sign");
        Actions.OnStartDay?.Invoke();
        signText.text = "Open";
        GetComponent<MeshRenderer>().material = openMaterial;
    }
}