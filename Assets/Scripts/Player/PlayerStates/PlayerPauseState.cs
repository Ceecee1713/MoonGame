using StateMachine;
using UnityEngine;

public class PlayerPauseState : PlayerState
{
    public PlayerPauseState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        Debug.Log("I'm in paused state"); 
    }
}
