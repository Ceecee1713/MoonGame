using StateMachine;
using UnityEngine;

/// <remarks>
/// This script inherits from "PlayerState" as the player uses a state machine and states
/// See <see cref="BaseStateMachine"/> for how the state machine is structured
/// See <see cref="PlayerState"/> for how each player state is structured
/// </remarks>

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {

    }
}
