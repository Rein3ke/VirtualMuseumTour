using UnityEngine;

namespace State
{
    public abstract class StateMachine : MonoBehaviour
    {
        public IState CurrentState { get; private set; }
        public IState PreviousState { get; private set; }

        private bool _inTransition = false;

        public void ChangeState(IState newState)
        {
            if (CurrentState == newState || _inTransition)
                return;

            ChangeStateRoutine(newState);
        }

        public void RevertState()
        {
            if (PreviousState != null)
                ChangeState(PreviousState);
        }

        private void ChangeStateRoutine(IState newState)
        {
            _inTransition = true;

            if (CurrentState != null)
                CurrentState.Exit();

            if (PreviousState != null)
                PreviousState = CurrentState;
            
            CurrentState = newState;
            
            if (CurrentState != null)
                CurrentState.Enter();
            
            _inTransition = false;
        }

        private void Update()
        {
            if (CurrentState != null && !_inTransition)
                CurrentState.Tick();
        }

        private void FixedUpdate()
        {
            if (CurrentState != null && !_inTransition)
                CurrentState.FixedTick();
        }
    }
}