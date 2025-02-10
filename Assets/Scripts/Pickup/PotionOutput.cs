using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionOutput : MonoBehaviour
{
    [SerializeField] private Renderer rend;
    private MaterialPropertyBlock propBlock;
    private Color storedColor;

    public RecipeSO potionInside;
    public bool givenToCustomer = false;

    private bool wasPaused;
    private bool addedWobble = false;

    Vector3 lastPos;
    Vector3 velocity;
    Vector3 lastRot;
    Vector3 angularVelocity;

    public float MaxWobble = 0.03f;
    public float WobbleSpeed = 1f;
    public float Recovery = 1f;

    private float wobbleAmountX;
    private float wobbleAmountZ;
    private float wobbleAmountToAddX;
    private float wobbleAmountToAddZ;
    private float pulse;
    private float time = 0.5f;


    public void Update()
    {
        if(wasPaused && Time.timeScale > 0)
        {
            wasPaused = false;
            SetPotionColor();
        }
        else if (Time.timeScale == 0)
        {
            wasPaused = true;
            addedWobble = false;
        }

        if (!wasPaused && addedWobble)
        {
            if (!wasPaused && addedWobble)
            {
                time += Time.deltaTime;

                // Decrease wobble over time
                wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, Time.deltaTime * Recovery);
                wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, Time.deltaTime * Recovery);

                // Create a sine wave wobble
                pulse = 2 * Mathf.PI * WobbleSpeed;
                wobbleAmountX = wobbleAmountToAddX * Mathf.Sin(pulse * time);
                wobbleAmountZ = wobbleAmountToAddZ * Mathf.Sin(pulse * time);

                if(float.IsNaN(wobbleAmountX) || float.IsNaN(wobbleAmountZ))
                {
                    wobbleAmountX = 0;
                    wobbleAmountZ = 0;

                    return;
                }

                // Apply rotation wobble instead of modifying the material
                transform.localRotation = Quaternion.Euler(
                    wobbleAmountX * 5f,   // Small tilt on X
                    transform.localRotation.eulerAngles.y, // Keep original Y rotation
                    wobbleAmountZ * 5f    // Small tilt on Z
                );

                // Calculate velocity changes
                velocity = (lastPos - transform.position) / Time.deltaTime;
                angularVelocity = transform.rotation.eulerAngles - lastRot;

                // Add clamped velocity to wobble
                wobbleAmountToAddX += Mathf.Clamp((velocity.x + (angularVelocity.z * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
                wobbleAmountToAddZ += Mathf.Clamp((velocity.z + (angularVelocity.x * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);

                // Keep last position and rotation
                lastPos = transform.position;
                lastRot = transform.rotation.eulerAngles;
            }

        }
    }

    public void SetPotionColor()
    {
        storedColor = potionInside.potionColor;
        ApplyColor();
    }

    private void ApplyColor()
    {
        propBlock = new MaterialPropertyBlock();
        rend.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_Fill", 0.6f);
        propBlock.SetColor("_Color", storedColor);
        rend.SetPropertyBlock(propBlock);

        addedWobble = true;
    }
}