using System;
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
        /*/// <summary>
        /// The default path to the prefabs in the Resources folder.
        /// </summary>
        private const string PrefabPath = "prefabs";*/
        
        /*
        private PlayerSpawnController _playerSpawnController;
        private ExhibitManager _exhibitManager;
        private ExhibitDetailsUserInterface _exhibitDetailsUserInterface;
        private NavigationUserInterface _navigationUserInterface;
        private SelectionManager _selectionManager;
        private AudioController _audioController;
        private SceneController _sceneController;
        */

        /*
        /// <summary>
        /// Loads all controller and UI prefabs to instantiate them.
        /// </summary>
        private void Start()
        {
            // ControllerSetup();
            InterfaceSetup();

            void ControllerSetup()
            {
                _playerSpawnController = Instantiate(LoadFromResourcesAsGameObject("PlayerSpawnController"), transform).GetComponent<PlayerSpawnController>();
                _exhibitManager = Instantiate(LoadFromResourcesAsGameObject("ExhibitManager"), transform).GetComponent<ExhibitManager>();
                _selectionManager = Instantiate(LoadFromResourcesAsGameObject("SelectionManager"), transform).GetComponent<SelectionManager>();
                _audioController = Instantiate(LoadFromResourcesAsGameObject("AudioController"), transform).GetComponent<AudioController>();
                _sceneController = Instantiate(LoadFromResourcesAsGameObject("SceneController"), transform).GetComponent<SceneController>();
            }
            
            void InterfaceSetup()
            {
                _exhibitDetailsUserInterface = Instantiate(LoadFromResourcesAsGameObject("ExhibitDetails_UserInterface"), transform).GetComponent<ExhibitDetailsUserInterface>();
                _navigationUserInterface = Instantiate(LoadFromResourcesAsGameObject("Navigation_UserInterface"), transform).GetComponent<NavigationUserInterface>();
            }
        }*/

        /*/// <summary>
        /// Takes the name of a prefab (e.g. "PlayerSpawnController"), loads the associated prefab and finally returns it.
        /// </summary>
        /// <param name="prefabName">Name of the prefab as a string.</param>
        /// <returns>The corresponding prefab from the Resources folder.</returns>
        private static GameObject LoadFromResourcesAsGameObject(string prefabName)
        {
            return (GameObject) Resources.Load($"{PrefabPath}/{prefabName}");
        }*/
        public ApplicationState CurrentState { get; private set; }

        private AudioController _audioController;
        private SceneController _sceneController;
        private PlayerSpawnController _playerSpawnController;
        private LockStateManager _lockStateManager;
        private ExhibitManager _exhibitManager;
        private SelectionManager _selectionManager;

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

        public void SetState(ApplicationState state)
        {
            Debug.Log($"Previous State is {CurrentState}");
            
            switch (state)
            {
                case ApplicationState.Start:
                    CurrentState = state;
                    
                    // TODO: Add code ...
                    Debug.Log("Enter Start State");
                    SetState(ApplicationState.Menu);
                    return;
                case ApplicationState.Main:
                    CurrentState = state;
                    
                    // TODO: Add code ...
                    _audioController.StartEnvironmentLoop();
                    _audioController.StartMusicLoop();

                    break;
                case ApplicationState.Menu:
                    CurrentState = state;
                    
                    // TODO: Add code ...
                    _sceneController.LoadScene("Scenes/MainMenu");
                    _audioController.StopEnvironmentLoop();
                    _audioController.StartMusicLoop();
                    
                    break;
                case ApplicationState.PauseMenu:
                    CurrentState = state;
                    
                    // TODO: Add code ...
                    _audioController.StopEnvironmentLoop();
                    
                    break;
                default:
                    Debug.LogError("State not found!");
                    break;
            }

            Debug.Log($"New State is {CurrentState}");
            EventManager.TriggerEvent(EventType.EventStateChange, new EventParam
            {
                Param5 = state
            });
        }

        private void SetState(EventParam eventParam)
        {
            SetState(eventParam.Param5);
        }

        private void OnEnable()
        {
            EventManager.StartListening(EventType.EventSetState, SetState);
        }

        private void OnDisable()
        {
            EventManager.StopListening(EventType.EventSetState, SetState);
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
