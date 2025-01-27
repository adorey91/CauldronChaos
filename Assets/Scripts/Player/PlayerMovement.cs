using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement Variables")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 180;

    [Header("Object References")]
    [SerializeField] private Rigidbody playerRB;

    Vector2 moveDir = Vector2.zero;

    private Vector3 _currentVelocity = Vector3.zero;
    private float _iceFriction = 0.9f; // slows down gradually
    private bool isOnIce = false;

    //Called when object is enabled
    private void OnEnable()
    {
        InputManager.instance.MoveAction += GetMove;
    }

    //Called when object is disabled
    private void OnDisable()
    {
        InputManager.instance.MoveAction -= GetMove;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //translate Vector2 to Vector3
        Vector3 movement = new Vector3(moveDir.x, 0, moveDir.y).normalized * moveSpeed;

        //rotate towards direction
        if (movement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        isOnIce = IsOnIce();

        if (!isOnIce)
        {
            //apply multipliers
            movement *= Time.fixedDeltaTime;

            //apply movement
            playerRB.MovePosition(playerRB.position + movement);
        }
        else
        {
            if(movement.magnitude > 0)
            {
                _currentVelocity = movement * Time.fixedDeltaTime;
            }
            else
            {
                _currentVelocity *= _iceFriction;
            }

            _currentVelocity = Vector3.ClampMagnitude(_currentVelocity, moveSpeed);
            playerRB.MovePosition(playerRB.position + _currentVelocity);
        }
    }

    private void GetMove(InputAction.CallbackContext input)
    {
        moveDir = input.ReadValue<Vector2>();
        //Debug.Log("Get Move being called");
    }

    private bool IsOnIce()
    {
        Vector3 position = transform.position + Vector3.down;

        float radius = 0.2f;

        // Check all colliders in the sphere
        Collider[] colliders = Physics.OverlapSphere(position, radius, LayerMask.GetMask("Ice"));

        // If we find any colliders in the Ice layer, we're on ice
        return colliders.Length > 0;
    }
}
