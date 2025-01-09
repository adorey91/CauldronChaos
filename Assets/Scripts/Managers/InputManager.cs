using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    //static variable holding an instance of the InputManager;
    private static InputManager _instance;

    //input variables
    private InputAction.CallbackContext moveInput;
    private InputAction.CallbackContext interactInput;
    private InputAction.CallbackContext pickupInput;

    //function that checks if instance exists and spawns one if it does not
    public static InputManager instance
    {
        get
        {
            //check if instance is null
            if (_instance == null)
            {
                //spawn instance
                _instance = Instantiate(Resources.Load("InputManager") as GameObject).GetComponent<InputManager>();
                _instance.name = "InputManager"; //renames the game object to InputManager
            }
            return _instance; //returns 
        }
    }

    // Awake is called before the first frame update and before start
    void Awake()
    {
        //check if this is the active instance
        if (!_instance || _instance == this)
        {
            _instance = this;
        }
        else
        {
            //remove copy
            Destroy(gameObject);
        }

        //add to don't destroy on load
        DontDestroyOnLoad(this);
    }

    //function that reads the move input
    public void MoveInput(InputAction.CallbackContext input)
    {
        moveInput = input;
    }

    //function that reads the interact input
    public void InteractInput(InputAction.CallbackContext input)
    {
        interactInput = input;
    }

    //function that reads the interact input
    public void PickupInput(InputAction.CallbackContext input)
    {
        pickupInput = input;
    }

    //function that returns the input for movement
    public InputAction.CallbackContext GetMoveInput()
    {
        return moveInput;
    }

    //function that returns the input for interact
    public InputAction.CallbackContext GetInteractInput()
    {
        return interactInput;
    }

    //function that returns the input for pickup
    public InputAction.CallbackContext GetPickupInput()
    {
        return pickupInput;
    }

}
