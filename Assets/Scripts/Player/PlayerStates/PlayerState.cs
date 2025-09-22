using StateMachine;
using UnityEngine;

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
