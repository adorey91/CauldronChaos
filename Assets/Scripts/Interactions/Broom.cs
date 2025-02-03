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
        Vector3 rotation;
        float timer = 0f;
        float inTime = swingTime * swingRatio; //calculating swing in time
        float outTime = swingTime * (1 - swingRatio); //calculation swing out time

        //swing in
        while (timer < inTime)
        {
            rotation = Vector3.right * Mathf.Lerp(0, swingAngle, Mathf.Pow(timer/inTime, 2)); //calculate angle in swing along exponential curve
            transform.parent.eulerAngles = rotation; //apply rotation

            yield return null;
            timer += Time.deltaTime; //increment timer
        }

        //snap to correct value
        transform.parent.eulerAngles = new Vector3(swingAngle, 0, 0);

        //apply swing functionality
        SwingFunc();

        timer = 0f; //reset timer

        //swing out
        while (timer < outTime)
        {
            rotation = Vector3.right * Mathf.SmoothStep(swingAngle, 0, timer/outTime); //calculate angle out of swing with damping
            transform.parent.eulerAngles = rotation; //apply rotation

            yield return null;
            timer += Time.deltaTime; //increment timer
        }

        //snap to correct value
        transform.parent.eulerAngles = new Vector3(0, 0, 0);
    }
}
