using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    //static variable holding an instance of the InputManager;
    private static InputManager _instance;

    //input actions
    public event Action<InputAction.CallbackContext> MoveAction;
    public event Action<InputAction.CallbackContext> InteractAction;
    public event Action<InputAction.CallbackContext> PickupAction;


    //function that checks if instance exists and spawns one if it does not
    // this is spawning a new input manager when quitting play
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

        // This isnt needed as it's now nested under the game manager.
        //DontDestroyOnLoad(this);
    }

    //function that reads the move input
    public void MoveInput(InputAction.CallbackContext input)
    {
        MoveAction?.Invoke(input);
    }

    //function that reads the interact input
    public void InteractInput(InputAction.CallbackContext input)
    {
        //interactInput = input;
        if(input.performed)
            Actions.OnInteract?.Invoke();
    }

    //function that reads the interact input
    public void PickupInput(InputAction.CallbackContext input)
    {
        //pickupInput = input;

        if (input.performed)
            Actions.OnPickup?.Invoke();
    }


    public void PauseInput(InputAction.CallbackContext input)
    {
        if (input.performed)
            Actions.OnPause?.Invoke();
    }

    public void StirClockwiseInput(InputAction.CallbackContext input)
    {
        if (input.performed)
            Actions.OnStirClockwise?.Invoke();
    }

    public void StirCounterClockwiseInput(InputAction.CallbackContext input)
    {
        if (input.performed)
            Actions.OnStirCounterClockwise?.Invoke();
    }

    public void TurnNextPage(InputAction.CallbackContext input)
    {
        if(input.performed)
            Actions.NextPage?.Invoke();
    }

    public void TurnPreviousPage(InputAction.CallbackContext input)
    {
        if (input.performed)
            Actions.PreviousPage?.Invoke();
    }
}
