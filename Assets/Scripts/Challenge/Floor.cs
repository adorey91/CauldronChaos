using UnityEngine;
using System;

public class Floor : MonoBehaviour
{
    #region OnEnable / OnDisable / OnDestroy Events
    private void OnEnable()
    {
        Actions.OnApplyFoorMaterial += ApplyMaterial;
    }

    private void OnDisable()
    {
        Actions.OnApplyFoorMaterial -= ApplyMaterial;
    }

    private void OnDestroy()
    {
        Actions.OnApplyFoorMaterial -= ApplyMaterial;
    }
    #endregion

    private void ApplyMaterial(PhysicMaterial material, Texture texture)
    {
        GetComponent<Collider>().material = material;
        GetComponent<Renderer>().material.mainTexture = texture;
    }
}
