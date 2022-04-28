using Controller;
using Events;
using State;
using UnityEngine;
using EventType = Events.EventType;

namespace Interface.UIStates
{
    /// <summary>
    /// This class (state) describes the behaviour when the navigation user interface is shown.
    /// </summary>
    public class InterfaceNavigationState : IState
    {
        #region Properties
        
        /// <summary>
        /// Reference to the current UIManager.
        /// </summary>
        private UIManager UIManager { get; }
        /// <summary>
        /// Reference to the current NavigationUserInterface instance.
        /// </summary>
        private NavigationUserInterface NavigationUserInterface { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor. Stores references to the UIManager and the NavigationUserInterface.
        /// </summary>
        /// <param name="uiManager">Current UIManager reference.</param>
        /// <param name="navigationUserInterface">Current NavigationUserInterface reference.</param>
        public InterfaceNavigationState(UIManager uiManager, NavigationUserInterface navigationUserInterface)
        {
            UIManager = uiManager;
            NavigationUserInterface = navigationUserInterface;
        }

        #endregion

        #region IState Implementation

        /// <summary>
        /// Subscribes to the ExhibitSelect and DollHouseView open event.
        /// If an exhibit is selected, the navigation user interface is closed and the details user interface is opened.
        /// If the doll house view is opened, the navigation user interface is closed.
        /// </summary>
        public void Enter()
        {
            EventManager.StartListening(EventType.EventExhibitSelect, ChangeToDetailsState);
            EventManager.StartListening(EventType.EventDollHouseView, ChangeToDollHouseState);
            NavigationUserInterface.ShowInterface();
            LockStateManager.SetInternLockState(CursorLockMode.None); // Unlock the cursor and make it visible.
        }
        
        /// <summary>
        /// Checks at each frame whether the user has clicked the right mouse button. If this is the case, the ChangeToEmptyState method is called.
        /// </summary>
        public void Tick()
        {
            if (Input.GetMouseButtonDown(1)) // On right mouse button click
            {
                ChangeToEmptyState();
            }
        }

        /// <summary>
        /// Unsubscribes from the events and hides the navigation user interface.
        /// </summary>
        public void Exit()
        {
            EventManager.StopListening(EventType.EventExhibitSelect, ChangeToDetailsState);
            EventManager.StopListening(EventType.EventDollHouseView, ChangeToDollHouseState);
            NavigationUserInterface.HideInterface();
            LockStateManager.SetInternLockState(CursorLockMode.None); // Unlock and display cursor
        }

        #endregion

        #region State Change Methods

        /// <summary>
        /// Orders the UIManager to change to the InterfaceDetailsState.
        /// </summary>
        /// <param name="eventParam">(Obsolete).</param>
        private void ChangeToDetailsState(EventParam eventParam)
        {
            UIManager.ChangeState(UIManager.DetailsState);
        }

        /// <summary>
        /// Orders the UIManager to change to the InterfaceDollHouseViewState.
        /// </summary>
        /// <param name="eventParam">Boolean, if the DollHouseView is active (via EventParam).</param>
        private void ChangeToDollHouseState(EventParam eventParam)
        {
            if (eventParam.EventBoolean) // if the DollHouseView is active
            {
                UIManager.ChangeState(UIManager.DollHouseViewState);
            }
        }

        /// <summary>
        /// Orders the UIManager to change to the InterfaceEmptyState.
        /// </summary>
        private void ChangeToEmptyState()
        {
            UIManager.ChangeState(UIManager.EmptyState);
        }

        #endregion
    }
}