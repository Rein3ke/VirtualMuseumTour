using UnityEngine;

namespace State
{
    /// <summary>
    /// Class that can be inherited by any other class that uses the State pattern.
    /// Stores the current state and has methods for changing the state.
    /// </summary>
    public abstract class StateMachine : MonoBehaviour
    {
        #region Properties

        /// <summary>
        /// Reference to the current state.
        /// </summary>
        protected IState CurrentState { get; private set; }

        #endregion

        #region Members

        /// <summary>
        /// Set to true if the state machine is in a transition.
        /// </summary>
        private bool _inTransition;

        #endregion

        #region Unity Methods

        /// <summary>
        /// Calls the Tick method of the current state if the state machine is not in a transition.
        /// </summary>
        private void Update()
        {
            if (CurrentState != null && !_inTransition)
                CurrentState.Tick();
        }

        #endregion

        #region State Change

        /// <summary>
        /// Calls ChangeStateRoutine to change the current state to the new state.
        /// </summary>
        /// <param name="newState">New State.</param>
        public void ChangeState(IState newState)
        {
            if (CurrentState == newState || _inTransition) return;

            ChangeStateRoutine(newState);
        }

        /// <summary>
        /// Changes the current state to the new state. Sets _inTransition to true while the state is changing.
        /// Calls the Exit method of the current state before the state is changed. Then calls the start method of the new state.
        /// </summary>
        /// <param name="newState"></param>
        private void ChangeStateRoutine(IState newState)
        {
            _inTransition = true;

            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState?.Enter();

            _inTransition = false;
        }

        #endregion
    }
}