using UnityEngine;

namespace StateMachine
{
    public interface BaseState
    {
        void Enter();
        void Update();
        void Exit();
        void FixedUpdate();
    }
}

