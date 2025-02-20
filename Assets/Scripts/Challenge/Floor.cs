using UnityEngine;
using System;

public class Floor : MonoBehaviour
{
    /// <summary> Event to apply material to the floor object </summary>
    public static Action <PhysicMaterial, Texture> OnApplyMaterial;

    private void OnEnable()
    {
        OnApplyMaterial += ApplyMaterial;
    }

    private void OnDisable()
    {
        OnApplyMaterial -= ApplyMaterial;
    }

    private void ApplyMaterial(PhysicMaterial material, Texture texture)
    {
        GetComponent<Collider>().material = material;
        GetComponent<Renderer>().material.mainTexture = texture;
    }
}
