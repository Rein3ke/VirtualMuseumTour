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
        
        private GameObject _playerSpawnControllerGameObject;
        private GameObject _exhibitManagerGameObject;
        private GameObject _exhibitDetailsUserInterfaceGameObject;
        private GameObject _navigationUserInterfaceGameObject;
        private GameObject _selectionManagerGameObject;
        private GameObject _audioControllerGameObject;

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
                _playerSpawnControllerGameObject = Instantiate(LoadFromResourcesAsGameObject("PlayerSpawnController"), transform);
                _exhibitManagerGameObject = Instantiate(LoadFromResourcesAsGameObject("ExhibitManager"), transform);
                _selectionManagerGameObject = Instantiate(LoadFromResourcesAsGameObject("SelectionManager"), transform);
                _audioControllerGameObject = Instantiate(LoadFromResourcesAsGameObject("AudioController"), transform);
            }
            
            void InterfaceSetup()
            {
                _exhibitDetailsUserInterfaceGameObject = Instantiate(LoadFromResourcesAsGameObject("ExhibitDetails_UserInterface"), transform);
                _navigationUserInterfaceGameObject = Instantiate(LoadFromResourcesAsGameObject("Navigation_UserInterface"), transform);
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
