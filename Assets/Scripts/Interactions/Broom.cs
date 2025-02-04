using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Broom : Interactable
{
    [Header("Broom Variables")]
    [SerializeField] private LayerMask interactionLayers;//layers that interact with the broom effect
    [SerializeField] private Transform impactPoint; //point at which the explosion force of the broom will be applied
    [SerializeField] private Animator animator; //animator used to make the broom movement
    [SerializeField] private float moveForce; //force applied to physics objects near the impact point
    [SerializeField] private float forceRadius; //radius in which the force will be applied

    public override void Interact()
    {
        animator.Play("BroomHit");
    }

    public override void Interact(PickupBehaviour pickup)
    {
        throw new System.NotImplementedException();
    }

    //Method that handles the functionality for the broom swing
    public void SwingFunc()
    {

        Collider[] colliders = Physics.OverlapSphere(impactPoint.position, forceRadius, interactionLayers); //get any colliders in the area
        //Debug.Log(colliders.Length);

        //loop through and apply force to nearby rigid bodies
        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>(); //try to get rigid body off colliders
            if (rb != null) //check if rigid body was found
            {
                rb.AddExplosionForce(moveForce, impactPoint.position, forceRadius); //apply the explosion "cleaning" force
            }
        }
    }
}
