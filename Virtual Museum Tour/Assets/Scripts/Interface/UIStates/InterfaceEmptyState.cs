using Controller;
using State;
using UnityEngine;

namespace Interface.UIStates
{
    /// <summary>
    /// This class (state) describes the behaviour of the UI if no interface is active.
    /// </summary>
    public class InterfaceEmptyState : IState
    {
        #region Properties

        /// <summary>
        /// Reference to the current UIManager.
        /// </summary>
        private UIManager UIManager { get; }
        /// <summary>
        /// Reference to the current ExhibitDetailsUserInterface instance.
        /// </summary>
        private ExhibitDetailsUserInterface ExhibitDetailsUserInterface { get; }
        /// <summary>
        /// Reference to the current NavigationUserInterface instance.
        /// </summary>
        private NavigationUserInterface NavigationUserInterface { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor. Stores references to the current UIManager, ExhibitDetailsUserInterface and the current NavigationUserInterface.
        /// </summary>
        /// <param name="uiManager">Current UIManager reference.</param>
        /// <param name="detailsUserInterface">Current ExhibitDetailsUserInterface reference.</param>
        /// <param name="navigationUserInterface">Current NavigationUserInterface reference.</param>
        public InterfaceEmptyState(UIManager uiManager, ExhibitDetailsUserInterface detailsUserInterface, NavigationUserInterface navigationUserInterface)
        {
            UIManager = uiManager;
            ExhibitDetailsUserInterface = detailsUserInterface;
            NavigationUserInterface = navigationUserInterface;
        }

        #endregion

        #region IState Implementation

        /// <summary>
        /// Hides all user interface elements and locks the mouse.
        /// </summary>
        public void Enter()
        {
            ExhibitDetailsUserInterface.HideInterface();
            NavigationUserInterface.HideInterface();
            LockStateManager.SetInternLockState(CursorLockMode.Locked); // Lock mouse cursor and make it invisible.
        }

        /// <summary>
        /// Checks at each frame whether the user has clicked the right mouse button. If this is the case, the ChangeToNavigationState method is called.
        /// </summary>
        public void Tick()
        {
            if (Input.GetMouseButtonDown(1)) // On right mouse button click
            {
                ChangeToNavigationState();
            }
        }

        /// <summary>
        /// (Gets called after exiting this state - Unused in this state)
        /// </summary>
        public void Exit()
        {
            
        }

        #endregion

        #region State Change Methods

        /// <summary>
        /// Uses the UIManager reference to switch to the NavigationState.
        /// </summary>
        private void ChangeToNavigationState()
        {
            UIManager.ChangeState(UIManager.NavigationState);
        }

        #endregion
    }
}