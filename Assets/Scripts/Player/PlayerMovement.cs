using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement Variables")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 180;

    [Header("Object References")]
    [SerializeField] private Rigidbody playerRB;

    Vector2 moveDir = Vector2.zero;

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
        Vector3 movement = new Vector3(moveDir.x, 0, moveDir.y);

        //rotate towards direction
        if (movement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime);
        }
        

        //apply multipliers
        movement *= moveSpeed * Time.fixedDeltaTime;

        //apply movement
        playerRB.MovePosition(playerRB.position + movement);
    }

    private void GetMove(InputAction.CallbackContext input)
    {
        moveDir = input.ReadValue<Vector2>();
        //Debug.Log("Get Move being called");
    }
}
