using UnityEngine;

namespace StateMachine
{
    public class BaseStateMachine : MonoBehaviour
    {
        protected BaseState CurrentState;
        protected string CurrentStateName;

        public virtual void ChangeState(BaseState newState)
        {
            if (newState == CurrentState)
                return;
                    
            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState?.Enter();
            CurrentStateName = CurrentState?.ToString();
        }

        public virtual void Update()
        {
            CurrentState?.Update();
        }
            
        public virtual void FixedUpdate()
        {
            CurrentState?.FixedUpdate();
        }
    }
}

