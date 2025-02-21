using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Image abovePlayerInteraction;
    [SerializeField] private TextMeshProUGUI abovePlayerInteractionText;

    //static variable holding an instance of the InputManager;
    private static InputManager _instance;

    //input actions
    public static Action<InputAction.CallbackContext> MoveAction;
    public static Action<InputAction.CallbackContext> InteractAction;
    public static Action<InputAction.CallbackContext> PickupAction;
    public static Action<InputAction.CallbackContext> StirClockwiseAction;
    public static Action<InputAction.CallbackContext> StirCounterClockwiseAction;
    public static Action<InputAction.CallbackContext> PauseAction;
    public static Action<InputAction.CallbackContext> NextPageAction;
    public static Action<InputAction.CallbackContext> PreviousPageAction;

    // actions for above player interaction
    public static Action OnInteract;
    public static Action OnPickup;
    public static Action OnStir;
    public static Action OnHide;

    private PlayerInput playerControls;
    private InputAction InteractInputAction;
    private InputAction PickupInputAction;
    private InputAction StirCAction;
    private InputAction StirCCAction;
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

        abovePlayerInteractionText.text = "";
    }

    private void Start()
    {
        playerControls = GetComponent<PlayerInput>();

        MoveInputAction = playerControls.actions.FindAction("Move");
        PauseInputAction = playerControls.actions.FindAction("Pause");
        NextPageInputAction = playerControls.actions.FindAction("Next Page");
        PreviousPageInputAction = playerControls.actions.FindAction("Previous Page");
        InteractInputAction = playerControls.actions.FindAction("Interact");
        PickupInputAction = playerControls.actions.FindAction("Pickup");
        StirCAction = playerControls.actions.FindAction("StirClockwise");
        StirCCAction = playerControls.actions.FindAction("StirCounterClockwise");

        PreviousPageInputAction.Disable();
        NextPageInputAction.Disable();
    }

    private void OnEnable()
    {
        Actions.OnEndDay += TurnOffInteraction;
        Actions.OnStartDay += TurnOnInteraction;
        OnInteract += ShowInteraction;
        OnPickup += ShowPickup;
        OnStir += ShowStir;
        OnHide+= HideInteractionPickup;
    }

    private void OnDisable()
    {
        Actions.OnEndDay -= TurnOffInteraction;
        Actions.OnStartDay -= TurnOnInteraction;
        OnInteract -= ShowInteraction;
        OnPickup -= ShowPickup;
        OnStir -= ShowStir;

        OnHide-= HideInteractionPickup;
    }

    #region Player Controls
    //function that reads the move input
    public void MoveInput(InputAction.CallbackContext input)
    {
        //if (Time.timeScale != 0)
        MoveAction?.Invoke(input);
    }

    //function that reads the interact input
    public void InteractInput(InputAction.CallbackContext input)
    {
        if (Time.timeScale != 0)
            InteractAction?.Invoke(input);
    }

    //function that reads the interact input
    public void PickupInput(InputAction.CallbackContext input)
    {
        if (Time.timeScale != 0)
            PickupAction?.Invoke(input);
    }


    public void PauseInput(InputAction.CallbackContext input)
    {
        PauseAction?.Invoke(input);
    }

    public void StirClockwiseInput(InputAction.CallbackContext input)
    {
        if (Time.timeScale != 0)
            StirClockwiseAction?.Invoke(input);
    }

    public void StirCounterClockwiseInput(InputAction.CallbackContext input)
    {
        if (Time.timeScale != 0)
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

    internal void TurnOffInteraction()
    {
        playerControls.SwitchCurrentActionMap("UI");
    }

    internal void TurnOnInteraction()
    {
        playerControls.SwitchCurrentActionMap("Player");
    }
    #endregion


    #region Above Player Interaction
    private void ShowInteraction()
    {
        abovePlayerInteraction.enabled = true;

        if(IsControllerConnected())
            abovePlayerInteractionText.text = InteractInputAction.GetBindingDisplayString(1);
        else
            abovePlayerInteractionText.text = InteractInputAction.GetBindingDisplayString(0);
    }

    private void ShowPickup()
    {
        abovePlayerInteraction.enabled = true;

        if (IsControllerConnected())
            abovePlayerInteractionText.text = PickupInputAction.GetBindingDisplayString(1);
        else
            abovePlayerInteractionText.text = PickupInputAction.GetBindingDisplayString(0);
    }

    private void ShowStir()
    {
        abovePlayerInteraction.enabled = true;
        if (IsControllerConnected())
            abovePlayerInteractionText.text = StirCAction.GetBindingDisplayString(1) + " / " + StirCCAction.GetBindingDisplayString(1);
        else
            abovePlayerInteractionText.text = StirCAction.GetBindingDisplayString(0) + " / " + StirCCAction.GetBindingDisplayString(0);
    }

    private void HideInteractionPickup()
    {
        abovePlayerInteraction.enabled = false;
        abovePlayerInteractionText.text = "";
    }

    private bool IsControllerConnected()
    {
        return Gamepad.all.Count > 0;
    }

    #endregion
}
