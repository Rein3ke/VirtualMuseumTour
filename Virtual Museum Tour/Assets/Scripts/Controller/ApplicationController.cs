using Audio;
using Events;
using SingletonPattern;
using UnityEngine;
using EventType = Events.EventType;

namespace Controller
{
    /// <summary>
    /// Controller, which manages the application state, as well as the references to other systems added as a component.
    /// </summary>
    [RequireComponent(typeof(AudioController))]
    [RequireComponent(typeof(SceneController))]
    [RequireComponent(typeof(PlayerSpawnController))]
    [RequireComponent(typeof(LockStateManager))]
    [RequireComponent(typeof(ExhibitManager))]
    [RequireComponent(typeof(SelectionManager))]
    public class ApplicationController : GenericSingletonClass<ApplicationController>
    {
        #region Members

        /// <summary>
        /// Reference to the AudioController component.
        /// </summary>
        private AudioController _audioController;
        /// <summary>
        /// Reference to the ExhibitManager component.
        /// </summary>
        private ExhibitManager _exhibitManager;
        /// <summary>
        /// Reference to the LockStateManager component.
        /// </summary>
        private LockStateManager _lockStateManager;
        /// <summary>
        /// Reference to the PlayerSpawnController component.
        /// </summary>
        private PlayerSpawnController _playerSpawnController;
        /// <summary>
        /// Reference to the SceneController component.
        /// </summary>
        private SceneController _sceneController;
        /// <summary>
        /// Reference to the SelectionManager component.
        /// </summary>
        private SelectionManager _selectionManager;

        #endregion

        #region Properties

        /// <summary>
        /// The current application state.
        /// </summary>
        public ApplicationState CurrentState { get; private set; }

        #endregion

        #region Unity Methods

        /// <summary>
        /// Gets the reference of all components and saves them.
        /// </summary>
        public override void Awake()
        {
            base.Awake();

            _audioController = GetComponent<AudioController>();
            _sceneController = GetComponent<SceneController>();
            _playerSpawnController = GetComponent<PlayerSpawnController>();
            _lockStateManager = GetComponent<LockStateManager>();
            _exhibitManager = GetComponent<ExhibitManager>();
            _selectionManager = GetComponent<SelectionManager>();
        }

        /// <summary>
        /// Initializes the application by setting the current state to start.
        /// </summary>
        private void Start()
        {
            SetState(ApplicationState.Start);
        }

        /// <summary>
        /// Subscribes to the EventSetState event, so that the application state can be changed by other systems.
        /// The SetState method is called by the EventSetState event.
        /// </summary>
        private void OnEnable()
        {
            EventManager.StartListening(EventType.EventSetState, SetState);
        }

        /// <summary>
        /// Unsubscribes from the EventSetState event.
        /// </summary>
        private void OnDisable()
        {
            EventManager.StopListening(EventType.EventSetState, SetState);
        }

        #endregion

        #region Application State Methods

        /// <summary>
        /// Sets the current application state and executes commands based on it.
        /// Afterwards the EventStateChanged event is fired.
        /// </summary>
        /// <param name="state">New application state.</param>
        private void SetState(ApplicationState state)
        {
            Debug.Log($"Previous State is {CurrentState}");
            
            switch (state)
            {
                case ApplicationState.Start:
                    CurrentState = state;
                    
                    SetState(ApplicationState.Menu); // switch to the menu state and return
                    return;
                case ApplicationState.Main:
                    CurrentState = state;
                    
                    _audioController.StartEnvironmentLoop();
                    _audioController.StartMusicLoop();

                    break;
                case ApplicationState.Menu:
                    CurrentState = state;
                    
                    _sceneController.LoadScene("Scenes/MainMenu");
                    _audioController.StopEnvironmentLoop();
                    _audioController.StartMusicLoop();
                    
                    break;
                case ApplicationState.PauseMenu:
                    CurrentState = state;
                    
                    _audioController.StopEnvironmentLoop();
                    
                    break;
                default:
                    Debug.LogError("State not found!");
                    break;
            }

            Debug.Log($"New State is {CurrentState}");
            EventManager.TriggerEvent(EventType.EventStateChange, new EventParam
            {
                EventApplicationState = state
            });
        }

        private void SetState(EventParam eventParam)
        {
            SetState(eventParam.EventApplicationState);
        }

        #endregion
    }

    /// <summary>
    /// Enum for the application states.
    /// </summary>
    public enum ApplicationState
    {
        Start,
        Main,
        Menu,
        PauseMenu
    }
}
