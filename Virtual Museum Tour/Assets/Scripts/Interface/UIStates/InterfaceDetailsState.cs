using Events;
using State;
using EventType = Events.EventType;

namespace Interface.UIStates
{
    public class InterfaceDetailsState : IState
    {
        private UIManager UIManager { get; }
        private ExhibitDetailsUserInterface ExhibitDetailsUserInterface { get; }
        
        public InterfaceDetailsState(UIManager uiManager, ExhibitDetailsUserInterface detailsUserInterface)
        {
            UIManager = uiManager;
            ExhibitDetailsUserInterface = detailsUserInterface;
        }

        public void Enter()
        {
            EventManager.StartListening(EventType.EventDetailsInterfaceClose, ChangeToNavigationState);
            ExhibitDetailsUserInterface.ShowInterface();
        }

        public void Tick()
        {
            
        }

        public void FixedTick()
        {
            
        }

        public void Exit()
        {
            EventManager.StopListening(EventType.EventDetailsInterfaceClose, ChangeToNavigationState);
            ExhibitDetailsUserInterface.HideInterface();
        }
        
        private void ChangeToNavigationState(EventParam eventParam)
        {
            UIManager.ChangeState(UIManager.NavigationState);
        }
    }
}