using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

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
    private Animator playerAnimation;
    private Vector2 moveDir = Vector2.zero;

    [Header("Ice Movement")]
    [SerializeField] private bool isOnIce = false;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float deceleration = 2f;
    [SerializeField] private float maxSpeed = 6f;

    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private bool canMove = false;

    public static Action<bool> OnIceDay;

    private void Awake()
    {
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
        playerAnimation = GetComponentInChildren<Animator>();
    }

    //Called when object is enabled
    private void OnEnable()
    {
        InputManager.MoveAction += GetMove;
        OnIceDay += ToggleIceMode;
        DayManager.OnStartDayCountdown += EnableMovement;
        Actions.OnEndDay += DisableMovement;
        Actions.OnResetValues += ResetPosition;
    }

    //Called when object is disabled
    private void OnDisable()
    {
        InputManager.MoveAction -= GetMove;
        OnIceDay -= ToggleIceMode;
        DayManager.OnStartDayCountdown -= EnableMovement;
        Actions.OnEndDay -= DisableMovement;
        Actions.OnResetValues -= ResetPosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(canMove)
        {
            if (!isOnIce)
                NormalMovement();
            else
                IceMovement();
        }
    }

    private void ResetPosition()
    {
        transform.position = spawnPosition;
        transform.rotation = spawnRotation;
        canMove = false;
    }

    private void EnableMovement()
    {
        canMove = true;
    }

    private void DisableMovement()
    {
        canMove = false;
    }

    public void ToggleIceMode(bool isIcy)
    {
        isOnIce = isIcy;

        // Reset velocity when switching back to normal movement
        if (!isIcy)
        {
            playerRB.velocity = Vector3.zero;
        }
    }

    private void GetMove(InputAction.CallbackContext input)
    {
        moveDir = input.ReadValue<Vector2>();
    }


    private void NormalMovement()
    {
        //translate Vector2 to Vector3
        Vector3 movement = new Vector3(moveDir.x, 0, moveDir.y);

        //rotate towards direction
        if (movement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime);
            playerAnimation.SetBool("isMoving", true);
        }
        else
        {
            playerAnimation.SetBool("isMoving", false);
        }

        //apply multipliers
        movement *= moveSpeed * Time.fixedDeltaTime;

        //handle collision detection
        movement = CheckMove(movement);

        //apply movement
        playerRB.MovePosition(playerRB.position + movement);

    }

    private void IceMovement()
    {
        Vector3 targetVelocity = new Vector3(moveDir.x, 0, moveDir.y) * maxSpeed;

        // Allow rotation even if standing still
        if (moveDir.sqrMagnitude > 0.001f)
        {
            // Rotate towards input direction
            Quaternion toRotation = Quaternion.LookRotation(targetVelocity.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime);
        }
        else if (playerRB.velocity.sqrMagnitude > 0.01f)
        {
            // Otherwise, rotate based on movement direction
            Quaternion toRotation = Quaternion.LookRotation(playerRB.velocity.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        // Handle acceleration and deceleration smoothly
        if (moveDir.sqrMagnitude > 0.001f)
        {
            playerRB.velocity = Vector3.MoveTowards(playerRB.velocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            playerAnimation.SetBool("isMoving", true);
        }
        else
        {
            playerRB.velocity = Vector3.MoveTowards(playerRB.velocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
            playerAnimation.SetBool("isMoving", playerRB.velocity.sqrMagnitude > 0.01f);
        }
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
