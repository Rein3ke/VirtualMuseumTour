using Audio;
using Events;
using SingletonPattern;
using UnityEngine;
using EventType = Events.EventType;

// Singleton
namespace Controller
{
    [RequireComponent(typeof(AudioController))]
    [RequireComponent(typeof(SceneController))]
    [RequireComponent(typeof(PlayerSpawnController))]
    [RequireComponent(typeof(LockStateManager))]
    [RequireComponent(typeof(ExhibitManager))]
    [RequireComponent(typeof(SelectionManager))]
    public class ApplicationController : GenericSingletonClass<ApplicationController>
    {
        private AudioController _audioController;
        private ExhibitManager _exhibitManager;
        private LockStateManager _lockStateManager;
        private PlayerSpawnController _playerSpawnController;
        private SceneController _sceneController;
        private SelectionManager _selectionManager;
        public ApplicationState CurrentState { get; private set; }

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

        private void Start()
        {
            SetState(ApplicationState.Start);
        }

        private void OnEnable()
        {
            EventManager.StartListening(EventType.EventSetState, SetState);
        }

        private void OnDisable()
        {
            EventManager.StopListening(EventType.EventSetState, SetState);
        }

        private void SetState(ApplicationState state)
        {
            Debug.Log($"Previous State is {CurrentState}");
            
            switch (state)
            {
                case ApplicationState.Start:
                    CurrentState = state;
                    
                    Debug.Log("Enter Start State");
                    SetState(ApplicationState.Menu);
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

        // ReSharper disable once UnusedMember.Global
        // Method is accessed by extern javascript.
        public void SetLockStateFromWeb(int lockMode)
        {
            if (lockMode > 2 || lockMode < 0) Debug.LogError("Invalid lockMode supplied.");
            // _lockStateManager.SetInternLockState((CursorLockMode) lockMode);
        }
    }

    public enum ApplicationState
    {
        Start,
        Main,
        Menu,
        PauseMenu
    }
}
