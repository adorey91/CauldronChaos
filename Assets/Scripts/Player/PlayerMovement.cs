using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement Variables")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 180;
    private float _defaultSpeed;

    [Header("Collision Detection")]
    [SerializeField] private Transform castPos; //position to do raycasts from
    [SerializeField] private float playerRadius; //radius of the player collision collider
    [SerializeField] private LayerMask collisionsLayers; //layers on which collisions happen

    [Header("Object References")]
    private Rigidbody _playerRb;
    private Animator _playerAnimation;
    private Vector2 _moveDir = Vector2.zero;

    [Header("Ice Movement")]
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float deceleration = 2f;
    [SerializeField] private float maxSpeed = 6f;
    private bool _isOnIce;

    [Header("Slime Movement")]
    [SerializeField] private LayerMask slime;
    [SerializeField] private float slowMultiplier = 0.5f;
    private bool _isInSlime;


    // Wind Movement
    private bool _isInWindZone;
    private Vector3 _windDirection;
    private WindyDay _windArea;
    private WindyDay.WindDirection _windDir;

    private Vector3 _spawnPosition;
    private Quaternion _spawnRotation;
    private bool _canMove;
    

    private void Awake()
    {
        _playerRb = GetComponent<Rigidbody>();
        _defaultSpeed = moveSpeed;
        _spawnPosition = transform.localPosition;
        _spawnRotation = transform.rotation;
        _playerAnimation = GetComponentInChildren<Animator>();


        if(SceneManager.GetActiveScene().name != "MainMenu")
        {
            _canMove = true;
        }
    }

    #region OnEnable / OnDisable / OnDestroy Events
    //Called when object is enabled
    private void OnEnable()
    {
        InputManager.MoveAction += GetMove;
        Actions.OnIceDay += ToggleIceMode;
        Actions.OnStartDay += EnableMovement;
        Actions.OnEndDay += DisableMovement;
        Actions.OnResetValues += ResetPosition;
    }

    //Called when object is disabled
    private void OnDisable()
    {
        InputManager.MoveAction -= GetMove;
        Actions.OnIceDay -= ToggleIceMode;
        Actions.OnStartDay -= EnableMovement;
        Actions.OnEndDay -= DisableMovement;
        Actions.OnResetValues -= ResetPosition;
    }

    private void OnDestroy()
    {
        InputManager.MoveAction -= GetMove;
        Actions.OnIceDay -= ToggleIceMode;
        Actions.OnStartDay -= EnableMovement;
        Actions.OnEndDay -= DisableMovement;
        Actions.OnResetValues -= ResetPosition;
    }
    #endregion

    private void Update()
    {
        // Check if the player is standing in a slime zone by raycasting downwards
        var isTouchingSlime = Physics.Raycast(transform.position, Vector3.down,1f, slime);

        switch (isTouchingSlime)
        {
            // If the player enters a slime zone
            case true when !_isInSlime:
                _isInSlime = true;
                // Apply the slow multiplier to the player's move speed
                moveSpeed *= slowMultiplier;
                break;
            // If the player is no longer in a slime zone
            case false when _isInSlime:
                _isInSlime = false;
                // Reset move speed to default
                moveSpeed = _defaultSpeed;
                break;
        }
    }

    private void FixedUpdate()
    {
        if (!_canMove) return;
        
        if(_isInWindZone)
            _playerRb.AddForce(_windDirection * _windArea.strength);

        if (!_isOnIce)
            NormalMovement();
        else
            IceMovement();
    }

    private void ResetPosition()
    {
        transform.localPosition = _spawnPosition;
        transform.rotation = _spawnRotation;
        _canMove = false;
    }

    private void EnableMovement()
    {
        _canMove = true;
    }

    private void DisableMovement()
    {
        _canMove = false;
    }

    private void ToggleIceMode(bool isIcy)
    {
        _isOnIce = isIcy;

        // Reset velocity when switching back to normal movement
        if (!isIcy)
        {
            _playerRb.velocity = Vector3.zero;
        }
    }

    private void GetMove(InputAction.CallbackContext input)
    {
        _moveDir = input.ReadValue<Vector2>();
    }


    private void NormalMovement()
    {
        //translate Vector2 to Vector3
        var movement = new Vector3(_moveDir.x, 0, _moveDir.y);

        //rotate towards direction
        if (movement != Vector3.zero)
        {
            var toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime);
            _playerAnimation.SetBool("isMoving", true);
        }
        else
        {
            _playerAnimation.SetBool("isMoving", false);
        }

        //apply multipliers
        movement *= moveSpeed * Time.fixedDeltaTime;

        //handle collision detection
        movement = CheckMove(movement);

        //apply movement
        _playerRb.MovePosition(_playerRb.position + movement);

    }

    private void IceMovement()
    {
        var targetVelocity = new Vector3(_moveDir.x, 0, _moveDir.y) * maxSpeed;

        // Allow rotation even if standing still
        if (_moveDir.sqrMagnitude > 0.001f)
        {
            // Rotate towards input direction
            var toRotation = Quaternion.LookRotation(targetVelocity.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime);
        }
        else if (_playerRb.velocity.sqrMagnitude > 0.01f)
        {
            // Otherwise, rotate based on movement direction
            var toRotation = Quaternion.LookRotation(_playerRb.velocity.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        // Handle acceleration and deceleration smoothly
        if (_moveDir.sqrMagnitude > 0.001f)
        {
            _playerRb.velocity = Vector3.MoveTowards(_playerRb.velocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            _playerAnimation.SetBool("isMoving", true);
        }
        else
        {
            _playerRb.velocity = Vector3.MoveTowards(_playerRb.velocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
            _playerAnimation.SetBool("isMoving", _playerRb.velocity.sqrMagnitude > 0.01f);
        }
    }



    //Function that tries to detect any collisions and modify the move to account for them
    private Vector3 CheckMove(Vector3 move)
    {
        Vector3 legalMove; //variable to store what version of the 

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

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("WindArea"))
        {
            _isInWindZone = true;
            _windArea = other.GetComponent<WindyDay>();
            _windDir = _windArea.windDirect;
            _windDirection = _windArea.direction;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(_windArea != null)
        {
            if(_windArea.windDirect != _windDir)
            {
                _windDirection = _windArea.direction;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("WindArea"))
        {
            _isInWindZone = false;
            _windArea = null;
        }
    }
}
