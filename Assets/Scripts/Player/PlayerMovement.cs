using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement Variables")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 180;

    [Header("Collision Detection")]
    [SerializeField] private Transform castPos; //position to do raycasts from
    [SerializeField] private float playerRadius; //radius of the player collision collider
    [SerializeField] private LayerMask collisionsLayers; //layers on which collisions happen

    [Header("Object References")]
    [SerializeField] private Rigidbody playerRB;

    Vector2 moveDir = Vector2.zero;

    //Called when object is enabled
    private void OnEnable()
    {
        InputManager.MoveAction += GetMove;
    }

    //Called when object is disabled
    private void OnDisable()
    {
        InputManager.MoveAction -= GetMove;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //translate Vector2 to Vector3
        Vector3 movement = new Vector3(moveDir.x, 0, moveDir.y);

        //rotate towards direction
        if (movement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        //apply multipliers
        movement *= moveSpeed * Time.fixedDeltaTime;

        //handle collision detection
        movement = CheckMove(movement);

        //apply movement
        playerRB.MovePosition(playerRB.position + movement);
    }


    private void GetMove(InputAction.CallbackContext input)
    {
        moveDir = input.ReadValue<Vector2>();
    }

    //Function that tries to detect any collisions and modify the move to account for them
    private Vector3 CheckMove(Vector3 move)
    {
        Vector3 legalMove = Vector3.zero; //varaible to store what version of the 

        //do initial check to see if move will collide with anything
        if (DetectCollisions(move))
        {
            //check move that uses only x component
            legalMove = new Vector3(move.x, 0f, 0f);
            if (DetectCollisions(legalMove))
            {
                //check move that uses only z component
                legalMove = new Vector3(0f, 0f, move.z);
                if (DetectCollisions(legalMove))
                {
                    //if collision is detected for both directions movement is stopped
                    legalMove = Vector3.zero;
                }
            }
        }
        //allow original move
        else
        {
            legalMove = move; 
        }

        return legalMove;
    }

    //Function that checks if the player will collide with anything during their move
    private bool DetectCollisions(Vector3 move)
    {
        return Physics.Raycast(castPos.position, move.normalized, move.magnitude + playerRadius, collisionsLayers);
    }
}
