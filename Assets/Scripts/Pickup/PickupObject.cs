using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PickupObject : MonoBehaviour
{
    private bool isHeld = false; //bool tracking if the pickup is held
    private Transform targetPos = null; //transform tarcking the target position of the pickup
    private Rigidbody rb; //rigidbody component of the pickup

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        //moves pickup towards the target position if the positions do not match
        if(isHeld && rb.position != targetPos.position)
        {
            rb.MovePosition(targetPos.position);
        }
    }

    //Function that picks up the pickup
    public void PickUp(Transform targetPos)
    {
        //setting held to true and removing gravity
        isHeld = true;
        rb.useGravity = false;

        //setting target position & parenting
        this.targetPos = targetPos;
        transform.position = targetPos.position;
        transform.parent = targetPos;
    }

    //Function that drops the pickup
    public void Drop()
    {
        //settomg held to false and enabling gravity
        isHeld = false;
        rb.useGravity = true;

        //removing target position & parent
        targetPos = null;
        transform.parent = null;
    }
}
