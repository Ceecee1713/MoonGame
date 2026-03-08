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
    private float timeDurationOfSpeedChangingForWalking = 4.0f; //Time duration to change player's speed in "x" amount of seconds, when player's walking
    [SerializeField]
    private float timeMultiplierForWalking = 3.0f; //How fast to change speed when player's walking

    [Header("Speed Up Values")] 
    [SerializeField]
    private float potionSpeed = 7.0f;
    [SerializeField]
    private float itemTimeDurationOfSpeedChanging = 4.0f; //Time duration to change player's speed in "x" amount of seconds, when player used speed potion 
    [SerializeField]
    private float itemTimeMultiplier = 3.0f; //How fast to change speed when a speed boost potion is used
    public float MaxLengthOfTimeForSpeedUp = 7.0f;
    public float CurrentTimeLengthForSpeedUp; //Displaying the time duration for however long the player is sped up, counting down to 0f

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
    private float _timeDurationOfSpeedChanging; //Time duration to change player's speed in "x" amount of seconds
    private float _timeMultiplierForMovementChanging; //How fast to change speed from 0f to "maximumSpeed"

    private bool _speedUpPlayer = false;
    private bool _hasPlayerTakenSpeedPotion = false;

    public PlayerState currentState { get; set; }
    public PlayerState PreviousState { get; set; }
    //public string currentStateName;

    //Specific single state scripts
    public PlayerIdleState IdleState { get; private set; }
    public PlayerWanderState WanderState { get; private set; }
    public PlayerPauseState PausedState { get; private set; }

    private Vector3 _moonStatuePosition;

    void Awake()
    {
        //Instantiating all specific state scripts
        IdleState = new PlayerIdleState(this);
        WanderState = new PlayerWanderState(this);
        PausedState = new PlayerPauseState(this);

        //Setting Speed Values for walking speed
        MovementSpeed = 0.0f;
        maximumSpeed = walkingSpeed;
        _timeDurationOfSpeedChanging = timeDurationOfSpeedChangingForWalking;
        _timeMultiplierForMovementChanging = timeMultiplierForWalking;

        _moonStatuePosition = new Vector3 (this.gameObject.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z);

        _characterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        ChangeState(IdleState);
        currentState = IdleState;

        EventBus.Instance.Subscribe<FreezePlayer>(FreezePlayer);
        EventBus.Instance.Subscribe<SpeedUpPlayer>(AllowToSpeedUpPlayer);
        EventBus.Instance.Subscribe<NewExplorationPhase>(StartNewExplorationPhase);
    }

    private void StartNewExplorationPhase(NewExplorationPhase newExplorationPhase)
    {
        this.gameObject.transform.position = _moonStatuePosition; 
    }

    public void Move(Vector2 movement)
    {
        _playerMovement.x = movement.x;
        _playerMovement.y = movement.y;
        _playerDirection = new Vector3(_playerMovement.x, 0f, _playerMovement.y).normalized;
    }

    private void FreezePlayer(FreezePlayer freezePlayer)
    {
        if(freezePlayer.PausePlayerMovement == true)
            StateChange(PausedState);
        else
            StateChange(IdleState);
    }

    private void AllowToSpeedUpPlayer(SpeedUpPlayer speedUpPlayer)
    {
        _speedUpPlayer = true;
    }

    public override void Update()
    {
        Mathf.Clamp(CurrentTimeLengthForSpeedUp, 0.0f, MaxLengthOfTimeForSpeedUp);

        CheckToSpeedUpPlayer();

        if(currentState == IdleState && _playerDirection.magnitude >= _minimumMovementDistance)
            StateChange(WanderState);

        if(_hasPlayerTakenSpeedPotion == true)
            SpeedUpPlayer();

        base.Update();
    }

    private void CheckToSpeedUpPlayer()
    {
        if(_speedUpPlayer == true && _hasPlayerTakenSpeedPotion == false && CurrentTimeLengthForSpeedUp == 0.0f) 
        {
            CurrentTimeLengthForSpeedUp = MaxLengthOfTimeForSpeedUp;
            _hasPlayerTakenSpeedPotion = true;

            //Changing values for speed up coroutine
            _timeDurationOfSpeedChanging = itemTimeDurationOfSpeedChanging;
            _timeMultiplierForMovementChanging = itemTimeMultiplier;
            maximumSpeed = potionSpeed;

            if(currentState == WanderState)
                StartSpeedChange();
        }
    }

    public void SpeedUpPlayer()
    {
        if (_hasPlayerTakenSpeedPotion == true && CurrentTimeLengthForSpeedUp > 0)
            CurrentTimeLengthForSpeedUp -= Time.deltaTime;

        else 
        {
            //Reset to default values (walking speed)
            CurrentTimeLengthForSpeedUp = 0.0f;
            _speedUpPlayer = false;
            _hasPlayerTakenSpeedPotion = false;
            _timeDurationOfSpeedChanging = timeDurationOfSpeedChangingForWalking;
            _timeMultiplierForMovementChanging = timeMultiplierForWalking;
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

        if((PreviousState == WanderState || PreviousState == IdleState) && currentState == PausedState)
            StartCoroutine(SpeedChange(0.0f));
    }

    IEnumerator SpeedChange(float targetSpeed) 
    {
        float elapsedTime = 0;

        while(elapsedTime < _timeDurationOfSpeedChanging)
        {
            float timer = elapsedTime * _timeDurationOfSpeedChanging;

            //Using Mathf.Towards to get exact value of targetSpeed as lerp doesn't get the exact value
            MovementSpeed = Mathf.MoveTowards(MovementSpeed, targetSpeed, _timeMultiplierForMovementChanging  * elapsedTime); //Smoothly change value of "MovementSpeed" to targetSpeed
            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }
}
