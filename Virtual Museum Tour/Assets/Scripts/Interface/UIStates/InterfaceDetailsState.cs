using Events;
using State;
using EventType = Events.EventType;

namespace Interface.UIStates
{
    /// <summary>
    /// This class (state) describes the behaviour when the Details user interface is active.
    /// </summary>
    public class InterfaceDetailsState : IState
    {
        #region Properties

        /// <summary>
        /// Stores a reference to the UIManager to have access to its switch state method.
        /// </summary>
        private UIManager UIManager { get; }
        /// <summary>
        /// Stores a reference to the ExhibitDetailsUserInterface to have access to its hide and show methods.
        /// </summary>
        private ExhibitDetailsUserInterface ExhibitDetailsUserInterface { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Sets the current reference to the UIManager and the ExhibitDetailsUserInterface.
        /// </summary>
        /// <param name="uiManager">Current active UIManager.</param>
        /// <param name="detailsUserInterface">Current ExhibitDetailsUserInterface.</param>
        public InterfaceDetailsState(UIManager uiManager, ExhibitDetailsUserInterface detailsUserInterface)
        {
            UIManager = uiManager;
            ExhibitDetailsUserInterface = detailsUserInterface;
        }

        #endregion

        #region IState Implementation

        /// <summary>
        /// Subscribes to the DetailsUserInterface close event to switch to the NavigationState when triggered.
        /// Opens the DetailsUserInterface.
        /// </summary>
        public void Enter()
        {
            EventManager.StartListening(EventType.EventDetailsInterfaceClose, ChangeToNavigationState);
            ExhibitDetailsUserInterface.ShowInterface();
        }

        /// <summary>
        /// Update method. Gets called every frame but is not used in this state.
        /// </summary>
        public void Tick()
        {
            // Do nothing
        }

        /// <summary>
        /// Unsubscribes from events and hides the interface.
        /// </summary>
        public void Exit()
        {
            EventManager.StopListening(EventType.EventDetailsInterfaceClose, ChangeToNavigationState);
            ExhibitDetailsUserInterface.HideInterface();
        }

        #endregion

        #region State Change Methods

        /// <summary>
        /// Orders the UIManager to change to the navigation state.
        /// </summary>
        /// <param name="eventParam">(Obsolete).</param>
        private void ChangeToNavigationState(EventParam eventParam)
        {
            UIManager.ChangeState(UIManager.NavigationState);
        }

        #endregion
    }
}