using Controller;
using Events;
using Interface.UIStates;
using State;
using UnityEngine;
using EventType = Events.EventType;

namespace Interface
{
    /// <summary>
    /// The UIManager handles all UI states by inheriting from the StateManager.
    /// </summary>
    public class UIManager : StateMachine
    {
        #region Members
        /// <summary>
        /// Stores the reference to the NavigationUserInterface object.
        /// </summary>
        private NavigationUserInterface _navigationUserInterface;
        /// <summary>
        /// Stores the reference to the ExhibitDetailsUserInterface object.
        /// </summary>
        private ExhibitDetailsUserInterface _detailsUserInterface;
        /// <summary>
        /// The GUIStyle for the text element in the lower right corner.
        /// </summary>
        private GUIStyle _currentGUIStyle;
        
        #endregion

        #region States (Properties)

        /// <summary>
        /// Public reference to the InterfaceNavigationState object.
        /// </summary>
        public InterfaceNavigationState NavigationState { get; private set; }
        /// <summary>
        /// Public reference to the InterfaceDetailsState object.
        /// </summary>
        public InterfaceDetailsState DetailsState { get; private set; }
        /// <summary>
        /// Public reference to the InterfaceEmptyState object.
        /// </summary>
        public InterfaceEmptyState EmptyState { get; private set; }
        /// <summary>
        /// Public reference to the InterfaceDollHouseViewState object.
        /// </summary>
        public InterfaceDollHouseViewState DollHouseViewState { get; private set; }

        #endregion

        #region Unity Methods

        /// <summary>
        /// Initializes the UIManager. Instantiate the interfaces and declares the states.
        /// </summary>
        private void Awake()
        {
            _navigationUserInterface = Instantiate(Resources.Load<NavigationUserInterface>("Prefabs/Navigation_UserInterface"), transform);
            _detailsUserInterface = Instantiate(Resources.Load<ExhibitDetailsUserInterface>("Prefabs/ExhibitDetails_UserInterface"), transform);
            
            NavigationState = new InterfaceNavigationState(this, _navigationUserInterface);
            DetailsState = new InterfaceDetailsState(this, _detailsUserInterface);
            EmptyState = new InterfaceEmptyState(this, _detailsUserInterface, _navigationUserInterface);
            DollHouseViewState = new InterfaceDollHouseViewState(this);
        }

        /// <summary>
        /// Subscribe to the OnApplicationStateChange event to call the OnApplicationStateChange method.
        /// </summary>
        private void OnEnable()
        {
            EventManager.StartListening(EventType.EventStateChange, OnApplicationStateChange);
        }

        /// <summary>
        /// Unsubscribe from the OnApplicationStateChange event.
        /// </summary>
        private void OnDisable()
        {
            EventManager.StopListening(EventType.EventStateChange, OnApplicationStateChange);
        }

        /// <summary>
        /// Displays a small hint in the bottom left corner of the screen to guide the user.
        /// </summary>
        private void OnGUI()
        {
            if (_currentGUIStyle == null) InitGUIStyle();
            
            var text = "";

            if (CurrentState == EmptyState)
            {
                text = "Right click to open interface.";
            }

            if (CurrentState == NavigationState)
            {
                text = "Right click to close interface.\nLeft click to select an exhibit (if you are standing in front of it).";
            }
            
            // add label with text in the lower right corner of the screen and center the text inside
            GUI.Box(new Rect(Screen.width - 350, Screen.height - 100, 350, 100), text, _currentGUIStyle);
        }
        
        #endregion
        
        /// <summary>
        /// Sets the GUIStyle for the OnGUI method.
        /// </summary>
        private void InitGUIStyle()
        {
            _currentGUIStyle ??= new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                normal =
                {
                    textColor = Color.white
                },
                alignment = TextAnchor.LowerRight
            };
        }

        /// <summary>
        /// Gets called when the application state changes. Depending on the application state, the UIManager switches to the corresponding interface state.
        /// </summary>
        /// <param name="eventParam">Application state (in EventApplicationState)</param>
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