using UnityEngine;
using System;

public class Floor : MonoBehaviour
{
    private PhysicMaterial slipperyMaterial;
    public static Action <PhysicMaterial> OnApplyMaterial;

    private void OnEnable()
    {
        OnApplyMaterial += ApplyMaterial;
    }

    private void OnDisable()
    {
        OnApplyMaterial -= ApplyMaterial;
    }

    private void ApplyMaterial(PhysicMaterial material)
    {
        slipperyMaterial = material;
        GetComponent<Collider>().material = slipperyMaterial;
    }
}
