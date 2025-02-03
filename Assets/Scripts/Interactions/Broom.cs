using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Broom : Interactable
{
    [Header("Broom Variables")]
    [SerializeField] private Transform impactPoint; //point at which the explosion force of the broom will be applied
    [SerializeField] private float moveForce; //force applied to physics objects near the impact point
    [SerializeField] private float forceRadius; //radius in which the force will be applied
    [SerializeField] private float swingAngle; //angle which the swing reaches
    [SerializeField] private float swingTime; //total time spent in swing animation
    [SerializeField][Range(0.1f, 0.5f)] private float swingRatio; //ratio between time into swing to time out of swing

    public override void Interact()
    {
        StartCoroutine(SwingAnim());
    }

    public override void Interact(PickupBehaviour pickup)
    {
        throw new System.NotImplementedException();
    }

    //Method that handles the functionality for the broom swing
    private void SwingFunc()
    {
        Collider[] colliders = Physics.OverlapSphere(impactPoint.position, forceRadius); //get any colliders in the area
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

    //Coroutine that preforms an animation through code an applies force for "cleaning"
    private IEnumerator SwingAnim()
    {
        float rotation = 0f;//current rotated angle
        float lastRotation = 0f;//last rotated angle
        float requiredRotation;//rotation required to reach current rotations
        float timer = 0f;
        float inTime = swingTime * swingRatio; //calculating swing in time
        float outTime = swingTime * (1 - swingRatio); //calculation swing out time

        //swing in
        while (timer < inTime)
        {
            lastRotation = rotation;
            rotation = Mathf.Lerp(0, swingAngle, Mathf.Pow(timer/inTime, 2)); //calculate angle in swing along exponential curve
            requiredRotation = rotation - lastRotation; //calculate required roation to reach current angle
            Debug.Log(rotation);

            transform.parent.RotateAround(transform.parent.position, transform.parent.right, requiredRotation); //apply rotations

            yield return null;
            timer += Time.deltaTime; //increment timer
        }

        //snap to correct value
        //transform.parent.localEulerAngles = transform.parent.forward * swingAngle;

        //apply swing functionality
        SwingFunc();

        //resetting varaibles
        timer = 0f; 
        rotation = 0f;
        lastRotation = 0f;

        //swing out
        while (timer < outTime)
        {
            lastRotation = rotation;
            rotation = -Mathf.SmoothStep(swingAngle, 0, timer/outTime); //calculate angle out of swing with damping
            requiredRotation = rotation - lastRotation; //calculate required roation to reach current angle
            Debug.Log(rotation);

            transform.parent.RotateAround(transform.parent.position, transform.parent.right, requiredRotation); //apply rotations

            yield return null;
            timer += Time.deltaTime; //increment timer
        }

        //snap to correct value
        //transform.parent.RotateAround;
    }
}
