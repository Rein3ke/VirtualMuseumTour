using System;
using Controller;
using Events;
using Interface.UIStates;
using State;
using UnityEngine;
using EventType = Events.EventType;

namespace Interface
{
    public class UIManager : StateMachine
    {
        #region Members

        private NavigationUserInterface _navigationUserInterface;
        private ExhibitDetailsUserInterface _detailsUserInterface;

        #endregion

        #region States (as properties)

        public InterfaceNavigationState NavigationState { get; private set; }
        public InterfaceDetailsState DetailsState { get; private set; }
        public InterfaceEmptyState EmptyState { get; private set; }
        
        public InterfaceDollHouseViewState DollHouseViewState { get; private set; }

        #endregion

        private void Awake()
        {
            _navigationUserInterface = Instantiate(Resources.Load<NavigationUserInterface>("Prefabs/Navigation_UserInterface"), transform);
            _detailsUserInterface = Instantiate(Resources.Load<ExhibitDetailsUserInterface>("Prefabs/ExhibitDetails_UserInterface"), transform);
            
            NavigationState = new InterfaceNavigationState(this, _navigationUserInterface);
            DetailsState = new InterfaceDetailsState(this, _detailsUserInterface);
            EmptyState = new InterfaceEmptyState(this, _detailsUserInterface, _navigationUserInterface);
            DollHouseViewState = new InterfaceDollHouseViewState(this);
        }

        private void OnEnable()
        {
            EventManager.StartListening(EventType.EventStateChange, OnApplicationStateChange);
        }

        private void OnDisable()
        {
            EventManager.StopListening(EventType.EventStateChange, OnApplicationStateChange);
        }

        private void OnApplicationStateChange(EventParam eventParam)
        {
            var applicationState = eventParam.EventApplicationState;
            if (applicationState == ApplicationState.Main)
            {
                _navigationUserInterface.gameObject.SetActive(true);
                _detailsUserInterface.gameObject.SetActive(true);
                ChangeState(EmptyState);
            }
            else
            {
                _navigationUserInterface.gameObject.SetActive(false);
                _detailsUserInterface.gameObject.SetActive(false);
            }
        }
    }
}