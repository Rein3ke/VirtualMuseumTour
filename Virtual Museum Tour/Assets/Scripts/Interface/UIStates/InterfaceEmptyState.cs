using Events;
using State;

namespace Interface.UIStates
{
    public class InterfaceEmptyState : IState
    {
        private UIManager UIManager { get; }

        public InterfaceEmptyState(UIManager uiManager)
        {
            UIManager = uiManager;
        }

        public void Enter()
        {
            EventManager.StartListening(EventType.EventOpenDollHouseView, ChangeToNavigationState);
        }

        public void Tick()
        {
            
        }

        public void FixedTick()
        {
            
        }

        public void Exit()
        {
            EventManager.StopListening(EventType.EventOpenDollHouseView, ChangeToNavigationState);
        }

        private void ChangeToNavigationState(EventParam eventParam)
        {
            if (!eventParam.EventBoolean) // if event was triggered with boolean "false" (closed doll house view e.g.)
            {
                UIManager.ChangeState(UIManager.NavigationState);
            }
        }
    }
}