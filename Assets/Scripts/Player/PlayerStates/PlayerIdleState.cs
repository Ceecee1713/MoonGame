using StateMachine;
using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        Debug.Log("I'm in idle state"); 
    }

    public override void FixedUpdate()
    {
        /*
        if(StateMachine._playerDirection.magnitude >= StateMachine._minimumMovementDistance)
        {
            StateMachine.StateChange(StateMachine.WanderState);
            StateMachine.SpeedChange(StateMachine.MaximumSpeed);
        }
        */
    }
}
