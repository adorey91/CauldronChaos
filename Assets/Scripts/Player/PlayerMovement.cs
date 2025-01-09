using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement Variables")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Object References")]
    [SerializeField] private CharacterController playerController;

    private Vector2 moveDir;
    private Vector3 movement;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //get movement input from input manager
        moveDir = InputManager.instance.GetMoveInput().ReadValue<Vector2>();

        //translate Vector2 to Vector3
        movement = new Vector3(moveDir.x, 0, moveDir.y);

        //apply multipliers
        movement *= moveSpeed * Time.deltaTime;

        //apply movement
        playerController.Move(movement);
    }
}
