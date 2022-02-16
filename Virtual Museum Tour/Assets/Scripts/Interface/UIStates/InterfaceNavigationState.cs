using Events;
using State;
using EventType = Events.EventType;

namespace Interface.UIStates
{
    public class InterfaceNavigationState : IState
    {
        private UIManager UIManager { get; }
        private NavigationUserInterface NavigationUserInterface { get; }
        
        public InterfaceNavigationState(UIManager uiManager, NavigationUserInterface navigationUserInterface)
        {
            UIManager = uiManager;
            NavigationUserInterface = navigationUserInterface;
        }

        public void Enter()
        {
            EventManager.StartListening(EventType.EventExhibitSelect, ChangeToDetailsState);
            NavigationUserInterface.ShowInterface();
        }

        public void Tick()
        {
            
        }

        public void FixedTick()
        {
            
        }

        public void Exit()
        {
            EventManager.StopListening(EventType.EventExhibitSelect, ChangeToDetailsState);
            NavigationUserInterface.HideInterface();
        }
        
        private void ChangeToDetailsState(EventParam eventParam)
        {
            UIManager.ChangeState(UIManager.DetailsState);
        }
    }
}