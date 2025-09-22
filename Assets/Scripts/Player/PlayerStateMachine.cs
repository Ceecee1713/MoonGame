using System;
using System.Collections;
using System.Collections.Generic;
using StateMachine;
using UnityEngine;

public class PlayerStateMachine : BaseStateMachine
{
    public Transform mainCamera;
    public float MovementSpeed;
    public float RotationSpeed = 10.0f;

    [Header("Default Speed Values")]
    [SerializeField]
    private float walkingSpeed = 3.0f;
    [SerializeField]
    private float _defaultDurationOfSpeedChanging = 4.0f; //Time window in order to change speed 
    [SerializeField]
    private float _defaultSpeedOfMovementChanging = 3.0f; //How fast to change speed

    [Header("Potion Usage Speed Values")] //All values to be moved to an item usage
    [SerializeField]
    private float potionSpeed = 7.0f;
    [SerializeField]
    private float _itemDurationOfSpeedChanging = 4.0f; //Time window in order to change speed for when a speed boost potion is used
    [SerializeField]
    private float _itemSpeedOfMovementChanging = 3.0f; //How fast to change speed for when a speed boost potion is used
    public float _maxLengthOfTimeForSpeedUp = 7.0f;
    public float _currentTimeLengthForSpeedUp;
    private bool _hasPlayerTakenSpeedPotion = false;

    [HideInInspector]
    public CharacterController _characterController;
    
    [HideInInspector]
    public Vector3 _playerDirection;
    [HideInInspector]
    public Vector3 _movementDirection; 
    [HideInInspector]
    public Vector3 _targetRotationDirection;

    [HideInInspector]
    public float _minimumMovementDistance = 0.1f;

    private Vector2 _playerMovement; //Grab raw movement inputs

    private float maximumSpeed;
    private float _durationOfSpeedChanging; //Time window in order to change speed 
    private float _speedOfMovementChanging; //How fast to change speed


    public PlayerState currentState { get; set; }
    public PlayerState PreviousState { get; set; }
    //public string currentStateName;

    //Specific single state scripts
    public PlayerIdleState IdleState { get; private set; }
    public PlayerWanderState WanderState { get; private set; }
    public PlayerPauseState PausedState { get; private set; }

    void Awake()
    {
        //Disabling mouse cursor and locking it in one place
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //Instantiating all specific state scripts
        IdleState = new PlayerIdleState(this);
        WanderState = new PlayerWanderState(this);
        PausedState = new PlayerPauseState(this);

        //Setting Speed Values
        MovementSpeed = 0.0f;
        maximumSpeed = walkingSpeed;
        _durationOfSpeedChanging = _defaultDurationOfSpeedChanging;
        _speedOfMovementChanging = _defaultSpeedOfMovementChanging;

        _characterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        ChangeState(IdleState);
        currentState = IdleState;
    }

    public void Move(Vector2 movement)
    {
        _playerMovement.x = movement.x;
        _playerMovement.y = movement.y;
        _playerDirection = new Vector3(_playerMovement.x, 0f, _playerMovement.y).normalized;
    }

    public override void Update()
    {
        Mathf.Clamp(_currentTimeLengthForSpeedUp, 0.0f, _maxLengthOfTimeForSpeedUp);

        CheckForItemUsage();

        if(currentState == IdleState && _playerDirection.magnitude >= _minimumMovementDistance)
            StateChange(WanderState);

        if(_hasPlayerTakenSpeedPotion == true)
            SpeedUpPlayer();
    }

    public void CheckForItemUsage()
    {
        if(Input.GetKeyDown(KeyCode.F) && _hasPlayerTakenSpeedPotion == false && _currentTimeLengthForSpeedUp == 0.0f) //Change key binding to something else
        {
            _currentTimeLengthForSpeedUp = _maxLengthOfTimeForSpeedUp;

            //Changing values for speed up coroutine
            _durationOfSpeedChanging = _itemDurationOfSpeedChanging;
            _speedOfMovementChanging = _itemSpeedOfMovementChanging;
            maximumSpeed = potionSpeed;

            _hasPlayerTakenSpeedPotion = true;

            if(currentState == WanderState)
                StartSpeedChange();
        }
    }

    public void SpeedUpPlayer()
    {
        if (_hasPlayerTakenSpeedPotion == true && _currentTimeLengthForSpeedUp > 0)
            _currentTimeLengthForSpeedUp -= Time.deltaTime;

        else 
        {
            //Reset to default values
            _currentTimeLengthForSpeedUp = 0.0f;
            _hasPlayerTakenSpeedPotion = false;
            _durationOfSpeedChanging = _defaultDurationOfSpeedChanging;
            _speedOfMovementChanging = _defaultSpeedOfMovementChanging;
            maximumSpeed = walkingSpeed;

            if(currentState == WanderState)
                StartSpeedChange();
        }
    }

    public void StateChange(PlayerState nextState)
    {
        PreviousState = currentState;
        currentState = nextState;
        ChangeState(nextState);
        //currentStateName = currentState?.ToString();
        StartSpeedChange();
    }

    public void StartSpeedChange() 
    {
        StopAllCoroutines();
        
        if(PreviousState == IdleState && currentState == WanderState)
            StartCoroutine(SpeedChange(maximumSpeed));

        if(PreviousState == WanderState && currentState == IdleState)
            StartCoroutine(SpeedChange(0.0f));
    }

    IEnumerator SpeedChange(float targetSpeed) 
    {
        float elapsedTime = 0;

        while(elapsedTime < _durationOfSpeedChanging)
        {
            float timer = elapsedTime * _durationOfSpeedChanging;

            //Using Mathf.Towards here to reach the exact value of targetSpeed as lerp doesn't get the exact value
            MovementSpeed = Mathf.MoveTowards(MovementSpeed, targetSpeed, _speedOfMovementChanging  * elapsedTime); //Smoothly changing value of "MovementSpeed" to targetSpeed
            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }
}
