using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement Variables")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 180;

    [Header("Object References")]
    [SerializeField] private Rigidbody playerRB;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //get movement input from input manager
        Vector2 moveDir = InputManager.instance.GetMoveInput().ReadValue<Vector2>().normalized;

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
}
