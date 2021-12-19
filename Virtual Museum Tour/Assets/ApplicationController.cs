using UnityEngine;

// Singleton
public class ApplicationController : MonoBehaviour
{
    public static ApplicationController Instance { get; private set; }
    
    private GameObject _playerSpawnControllerGameObject;
    private GameObject _exhibitManagerGameObject;

    private const string PrefabPath = "prefabs";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    
    private void Start()
    {
        ControllerSetup();
    }

    private void ControllerSetup()
    {
        _playerSpawnControllerGameObject = Instantiate(LoadFromResourcesAsGameObject("PlayerSpawnController"), transform);
        _exhibitManagerGameObject = Instantiate(LoadFromResourcesAsGameObject("ExhibitManager"), transform);
    }

    private GameObject GetManagerGameObject()
    {
        var manager = GameObject.FindGameObjectWithTag("GameController");

        if (manager == null)
        {
            Debug.LogError("Can't find Game Controller game object in scene.");
            return null;
        }
        else
        {
            return manager;
        }
    }

    private static GameObject LoadFromResourcesAsGameObject(string prefab)
    {
        return Resources.Load($"{PrefabPath}/{prefab}") as GameObject;
    }
}
