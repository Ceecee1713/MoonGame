using StateMachine;
using UnityEngine;

/// <remarks>
/// This script inherits from "PlayerState" as the player uses a state machine and states
/// See <see cref="BaseStateMachine"/> for how the state machine is structured
/// See <see cref="PlayerState"/> for how each player state is structured
/// </remarks>

public class PlayerState : BaseState
{
    protected PlayerStateMachine StateMachine;

    public PlayerState(PlayerStateMachine stateMachine)
    {
        StateMachine = stateMachine;
    }

    public virtual void Enter()
    {
        StateMachine.ChangeState(this);
    }

    public virtual void Exit()
    {   
    }

    public virtual void Update()
    {
    }

    public virtual void FixedUpdate()
    {
    }
}
