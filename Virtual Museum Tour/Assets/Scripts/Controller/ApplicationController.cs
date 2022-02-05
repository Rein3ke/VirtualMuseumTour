using Interface;
using UnityEngine;

// Singleton
namespace Controller
{
    public class ApplicationController : MonoBehaviour
    {
        /// <summary>
        /// Public singleton instance. Instance of the script must exist only once.
        /// </summary>
        public static ApplicationController Instance { get; private set; }
        /// <summary>
        /// The default path to the prefabs in the Resources folder.
        /// </summary>
        private const string PrefabPath = "prefabs";
        
        private PlayerSpawnController _playerSpawnController;
        private ExhibitManager _exhibitManager;
        private ExhibitDetailsUserInterface _exhibitDetailsUserInterface;
        private NavigationUserInterface _navigationUserInterface;
        private SelectionManager _selectionManager;
        private AudioController _audioController;
        private SceneController _sceneController;

        /// <summary>
        /// Set Instance to this if Instance isn't null. Otherwise destroy gameObject.
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }
    
        /// <summary>
        /// Loads all controller and UI prefabs to instantiate them.
        /// </summary>
        private void Start()
        {
            ControllerSetup();
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
        }

        /// <summary>
        /// Takes the name of a prefab (e.g. "PlayerSpawnController"), loads the associated prefab and finally returns it.
        /// </summary>
        /// <param name="prefabName">Name of the prefab as a string.</param>
        /// <returns>The corresponding prefab from the Resources folder.</returns>
        private static GameObject LoadFromResourcesAsGameObject(string prefabName)
        {
            return (GameObject) Resources.Load($"{PrefabPath}/{prefabName}");
        }
    }
}
