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
            EventManager.StartListening(EventType.EventDollHouseView, ChangeToEmptyState);
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
            EventManager.StopListening(EventType.EventDollHouseView, ChangeToEmptyState);
            NavigationUserInterface.HideInterface();
        }

        private void ChangeToDetailsState(EventParam eventParam)
        {
            UIManager.ChangeState(UIManager.DetailsState);
        }

        private void ChangeToEmptyState(EventParam eventParam)
        {
            if (eventParam.EventBoolean)
            {
                UIManager.ChangeState(UIManager.EmptyState);
            }
        }
    }
}