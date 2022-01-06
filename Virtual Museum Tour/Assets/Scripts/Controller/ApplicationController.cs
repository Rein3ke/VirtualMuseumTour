using UnityEngine;

// Singleton
namespace Controller
{
    public class ApplicationController : MonoBehaviour
    {
        public static ApplicationController Instance { get; private set; }
    
        private GameObject _playerSpawnControllerGameObject;
        private GameObject _exhibitManagerGameObject;
        private GameObject _exhibitDetailsUserInterfaceGameObject;
        private GameObject _navigationUserInterfaceGameObject;
        private GameObject _selectionManagerGameObject;
        private GameObject _audioControllerGameObject;

        private const string PrefabPath = "prefabs";

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

        private static GameObject LoadFromResourcesAsGameObject(string prefab)
        {
            return (GameObject) Resources.Load($"{PrefabPath}/{prefab}");
        }
    }
}
