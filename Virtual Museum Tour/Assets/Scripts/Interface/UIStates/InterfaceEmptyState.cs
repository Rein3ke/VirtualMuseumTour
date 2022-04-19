using Controller;
using Events;
using State;
using UnityEngine;

namespace Interface.UIStates
{
    public class InterfaceEmptyState : IState
    {
        private UIManager UIManager { get; }
        private ExhibitDetailsUserInterface ExhibitDetailsUserInterface { get; }
        private NavigationUserInterface NavigationUserInterface { get; }

        public InterfaceEmptyState(UIManager uiManager, ExhibitDetailsUserInterface detailsUserInterface, NavigationUserInterface navigationUserInterface)
        {
            UIManager = uiManager;
            ExhibitDetailsUserInterface = detailsUserInterface;
            NavigationUserInterface = navigationUserInterface;
        }

        public void Enter()
        {
            // EventManager.StartListening(EventType.EventDollHouseView, ChangeToNavigationState);
            ExhibitDetailsUserInterface.HideInterface();
            NavigationUserInterface.HideInterface();
            LockStateManager.SetInternLockState(CursorLockMode.Locked);
        }

        public void Tick()
        {
            if (Input.GetMouseButtonDown(1))
            {
                ChangeToNavigationState();
            }
        }

        public void FixedTick()
        {
            
        }

        public void Exit()
        {
            // EventManager.StopListening(EventType.EventDollHouseView, ChangeToNavigationState);
        }

        private void ChangeToNavigationState(EventParam eventParam)
        {
            if (!eventParam.EventBoolean) // if event was triggered with boolean "false" (closed doll house view e.g.)
            {
                UIManager.ChangeState(UIManager.NavigationState);
            }
        }

        private void ChangeToNavigationState()
        {
            UIManager.ChangeState(UIManager.NavigationState);
        }
    }
}