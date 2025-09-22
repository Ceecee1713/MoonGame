using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    private PlayerStateMachine _PlayerStateMachine;
    private PlayerInputs _playerInputs;

    void Awake()
    {
        _PlayerStateMachine = GetComponent<PlayerStateMachine>();
    }

    void OnEnable()
    {
        if(_playerInputs == null)
            _playerInputs = new PlayerInputs();

        _playerInputs.PlayerActions.Movement.performed += OnMovementPerformed;
        _playerInputs.PlayerActions.LookAround.performed += OnLookAroundPerformed;
        _playerInputs.PlayerActions.Interact.performed += OnInteractPerformed;
        _playerInputs.PlayerActions.Drop.performed += OnDropPerformed;
        _playerInputs.PlayerActions.Exit.performed += OnExitPerformed;

        _playerInputs.Enable();
    }

    void OnDisable()
    {
        _playerInputs.PlayerActions.Movement.performed -= OnMovementPerformed;
        _playerInputs.PlayerActions.LookAround.performed -= OnLookAroundPerformed;
        _playerInputs.PlayerActions.Interact.performed -= OnInteractPerformed;
        _playerInputs.PlayerActions.Drop.performed -= OnDropPerformed;
        _playerInputs.PlayerActions.Exit.performed -= OnExitPerformed;

        _playerInputs.Disable();
    }

    void OnMovementPerformed(InputAction.CallbackContext val)
    {
        _PlayerStateMachine.Move(val.ReadValue<Vector2>());
    }

    void OnLookAroundPerformed(InputAction.CallbackContext val)
    {
        
    }

    void OnInteractPerformed(InputAction.CallbackContext val)
    {

    }

    void OnDropPerformed(InputAction.CallbackContext val)
    {

    }

    void OnExitPerformed(InputAction.CallbackContext val)
    {

    }
}
