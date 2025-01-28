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
    public event Action<InputAction.CallbackContext> StirClockwiseAction;
    public event Action<InputAction.CallbackContext> StirCounterClockwiseAction;
    public event Action<InputAction.CallbackContext> PauseAction;
    public event Action<InputAction.CallbackContext> NextPageAction;
    public event Action<InputAction.CallbackContext> PreviousPageAction;

    private PlayerInput playerControls;
    internal InputAction MoveInputAction;
    internal InputAction PauseInputAction;
    internal InputAction NextPageInputAction;
    internal InputAction PreviousPageInputAction;


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

    private void Start()
    {
        playerControls = GetComponent<PlayerInput>();

        MoveInputAction =  playerControls.actions.FindAction("Move");
        MoveInputAction = playerControls.actions.FindAction("Move");
        PauseInputAction = playerControls.actions.FindAction("Pause");
        NextPageInputAction = playerControls.actions.FindAction("NextPage");
        PreviousPageInputAction = playerControls.actions.FindAction("PreviousPage");

        PreviousPageInputAction.Disable();
        NextPageInputAction.Disable();
    }


    //function that reads the move input
    public void MoveInput(InputAction.CallbackContext input)
    {
        MoveAction?.Invoke(input);
    }

    //function that reads the interact input
    public void InteractInput(InputAction.CallbackContext input)
    {
        InteractAction?.Invoke(input);
    }

    //function that reads the interact input
    public void PickupInput(InputAction.CallbackContext input)
    {
        PickupAction?.Invoke(input);
    }


    public void PauseInput(InputAction.CallbackContext input)
    {
        PauseAction?.Invoke(input);
    }

    public void StirClockwiseInput(InputAction.CallbackContext input)
    {
        StirClockwiseAction?.Invoke(input);
    }

    public void StirCounterClockwiseInput(InputAction.CallbackContext input)
    {
        StirCounterClockwiseAction?.Invoke(input);
    }

    public void TurnNextPage(InputAction.CallbackContext input)
    {
        NextPageAction?.Invoke(input);
    }

    public void TurnPreviousPage(InputAction.CallbackContext input)
    {
        PreviousPageAction?.Invoke(input);
    }
}
