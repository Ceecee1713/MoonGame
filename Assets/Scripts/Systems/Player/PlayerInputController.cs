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

    void Start()
    {
        EventBus.Instance.Subscribe<StopAllPlayerInputs>(ReachedEndScreen);
    }

    void OnEnable()
    {
        if(_playerInputs == null)
            _playerInputs = new PlayerInputs();

        _playerInputs.PlayerActions.Movement.performed += OnMovementPerformed;
        _playerInputs.PlayerActions.Interact.performed += OnInteractPerformed;
        _playerInputs.PlayerActions.Use.performed += OnUsePerformed;
        _playerInputs.PlayerActions.Drop.performed += OnDropPerformed;
        _playerInputs.PlayerActions.Exit.performed += OnExitPerformed;
        _playerInputs.PlayerActions.NextMessage.performed += NextMessagePerformed;

        _playerInputs.Enable();
    }

    void OnDisable()
    {
        UnsubscribeInputs();
    }

    private void UnsubscribeInputs()
    {
        _playerInputs.PlayerActions.Movement.performed -= OnMovementPerformed;
        _playerInputs.PlayerActions.Interact.performed -= OnInteractPerformed;
        _playerInputs.PlayerActions.Use.performed -= OnUsePerformed;
        _playerInputs.PlayerActions.Drop.performed -= OnDropPerformed;
        _playerInputs.PlayerActions.Exit.performed -= OnExitPerformed;
        _playerInputs.PlayerActions.NextMessage.performed -= NextMessagePerformed;
        _playerInputs.Disable();
    }

    private void ReachedEndScreen(StopAllPlayerInputs stopAllPlayerInputs)
    {
        UnsubscribeInputs();
    }

    void OnMovementPerformed(InputAction.CallbackContext val)
    {
        _PlayerStateMachine.Move(val.ReadValue<Vector2>());
    }

    void OnInteractPerformed(InputAction.CallbackContext val)
    {
        EventBus.Instance.Publish(new Interact());
    }

    void NextMessagePerformed(InputAction.CallbackContext val)
    {
        EventBus.Instance.Publish(new AdvanceThroughTextAdventure());
        EventBus.Instance.Publish(new AdvanceDialogueOnMainUI());
    }

    void OnUsePerformed(InputAction.CallbackContext val)
    {
        EventBus.Instance.Publish(new UseInventoryItem());
    }

    void OnDropPerformed(InputAction.CallbackContext val)
    {
        EventBus.Instance.Publish(new DropEquipedInventoryItem());
    }

    void OnExitPerformed(InputAction.CallbackContext val)
    {
        EventBus.Instance.Publish(new PauseGame());
    }
}
