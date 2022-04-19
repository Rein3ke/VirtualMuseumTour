using Controller;
using Events;
using State;
using UnityEngine;
using UnityEngine.EventSystems;
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
            EventManager.StartListening(EventType.EventDollHouseView, ChangeToDollHouseState);
            NavigationUserInterface.ShowInterface();
            LockStateManager.SetInternLockState(CursorLockMode.None);
        }

        public void Tick()
        {
            if (Input.GetMouseButtonDown(1))
            {
                ChangeToEmptyState();
            }
        }

        public void FixedTick()
        {
            
        }

        public void Exit()
        {
            EventManager.StopListening(EventType.EventExhibitSelect, ChangeToDetailsState);
            EventManager.StopListening(EventType.EventDollHouseView, ChangeToDollHouseState);
            NavigationUserInterface.HideInterface();
            LockStateManager.SetInternLockState(CursorLockMode.None);
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
        
        private void ChangeToDollHouseState(EventParam eventParam)
        {
            if (eventParam.EventBoolean)
            {
                UIManager.ChangeState(UIManager.DollHouseViewState);
            }
        }

        private void ChangeToEmptyState()
        {
            UIManager.ChangeState(UIManager.EmptyState);
        }
    }
}