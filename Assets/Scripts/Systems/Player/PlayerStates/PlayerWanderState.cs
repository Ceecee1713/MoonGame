using StateMachine;
using UnityEngine;

/// <remarks>
/// This script inherits from "PlayerState" as the player uses a state machine and states
/// See <see cref="BaseStateMachine"/> for how the state machine is structured
/// See <see cref="PlayerState"/> for how each player state is structured
/// </remarks>

public class PlayerWanderState : PlayerState
{
    public PlayerWanderState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {

    }

    public override void Update()
    {
        //StateMachine.PlayerDirection = new Vector3(StateMachine._playerMovement.x, 0f, StateMachine._playerMovement.y).normalized;

        if(StateMachine.PlayerDirection.magnitude >= StateMachine.MinimumMovementDistance)
        {
            //Movement relative to camera
            Vector3 cameraForward = StateMachine.mainCamera.forward;
            Vector3 cameraRight = StateMachine.mainCamera.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;
            cameraForward.Normalize();
            cameraRight.Normalize();
            
            StateMachine.MovementDirection = cameraForward * StateMachine.PlayerDirection.z + cameraRight * StateMachine.PlayerDirection.x;

            float yBefore = StateMachine.transform.position.y;

            StateMachine.CharacterController.Move(StateMachine.MovementDirection.normalized * StateMachine.MovementSpeed * Time.deltaTime);

            //Preventing Player's Y position from changing when rubbing against objects while moving
            Vector3 position = StateMachine.transform.position; 
            if (position.y != yBefore)
                StateMachine.transform.position = new Vector3(position.x, yBefore, position.z);

            //Player Rotation
            StateMachine.TargetRotationDirection = StateMachine.mainCamera.forward * StateMachine.PlayerDirection.z;
            StateMachine.TargetRotationDirection = StateMachine.TargetRotationDirection + StateMachine.mainCamera.right * StateMachine.PlayerDirection.x;
            StateMachine.TargetRotationDirection.Normalize();
            StateMachine.TargetRotationDirection.y = 0f;

            Quaternion targetRotation = Quaternion.LookRotation(StateMachine.TargetRotationDirection);
            Quaternion playerRotation = Quaternion.Slerp(StateMachine.transform.rotation, targetRotation, StateMachine.RotationSpeed * Time.deltaTime);

            StateMachine.transform.rotation = playerRotation;
        }

        else
            StateMachine.StateChange(StateMachine.IdleState);
    }
}

//Movement using Math and Angles
//float targetAngle = Mathf.Atan2(StateMachine.PlayerDirection.x, StateMachine.PlayerDirection.z) * Mathf.Rad2Deg + StateMachine.mainCamera.eulerAngles.y;
//StateMachine.MovementDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
//StateMachine._characterController.Move(StateMachine.MovementDirection.normalized * StateMachine.MovementSpeed * Time.deltaTime);
