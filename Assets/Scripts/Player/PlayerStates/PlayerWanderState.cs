using StateMachine;
using UnityEngine;

public class PlayerWanderState : PlayerState
{
    public PlayerWanderState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        Debug.Log("I'm in wander state"); 
    }

    public override void FixedUpdate()
    {
        //StateMachine._playerDirection = new Vector3(StateMachine._playerMovement.x, 0f, StateMachine._playerMovement.y).normalized;

        if(StateMachine._playerDirection.magnitude >= StateMachine._minimumMovementDistance)
        {
            //Player Movement
            float targetAngle = Mathf.Atan2(StateMachine._playerDirection.x, StateMachine._playerDirection.y) * Mathf.Rad2Deg + StateMachine.mainCamera.eulerAngles.y;
            StateMachine._movementDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            StateMachine._characterController.Move(StateMachine._movementDirection.normalized * StateMachine.MovementSpeed * Time.deltaTime);

            //Player Rotation
            StateMachine._targetRotationDirection = StateMachine.mainCamera.forward * StateMachine._playerDirection.z;
            StateMachine._targetRotationDirection = StateMachine._targetRotationDirection + StateMachine.mainCamera.right * StateMachine._playerDirection.x;
            StateMachine._targetRotationDirection.Normalize();
            StateMachine._targetRotationDirection.y = 0f;

            Quaternion targetRotation = Quaternion.LookRotation(StateMachine._targetRotationDirection);
            Quaternion playerRotation = Quaternion.Slerp(StateMachine.transform.rotation, targetRotation, StateMachine.RotationSpeed * Time.deltaTime);

            StateMachine.transform.rotation = playerRotation;
        }

        else
            StateMachine.StateChange(StateMachine.IdleState);
    }
}
