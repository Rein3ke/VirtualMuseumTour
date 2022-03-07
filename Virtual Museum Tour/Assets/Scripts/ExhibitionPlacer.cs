using System.Collections.Generic;
using Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using EventType = Events.EventType;

public class ExhibitionPlacer : MonoBehaviour
{
    [SerializeField] private GameObject[] exhibitionList;
    [SerializeField] private Vector3 origin;

    private bool _isPlaced;

    private void Start()
    {
        if (exhibitionList.Length == 0)
        {
            Debug.LogWarning("Caution. The exhibition list is empty. Remember to fill the list in the inspector with at least one exhibition prefab before starting the application.");
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneManager_OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneManager_OnSceneLoaded;
    }

    private void SceneManager_OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("ExhibitionMainScene"))
        {
            PlaceExhibitionsInGrid(exhibitionList);
        }
    }

    private void PlaceExhibitionsInGrid(IReadOnlyList<GameObject> exhibitions)
    {
        if (_isPlaced)
        {
            Debug.LogWarning("Exhibitions are already placed!");
            return;
        }
        
        if (exhibitions.Count <= 0)
        {
            Debug.LogError("No exhibitions found in list");
            return;
        }
        
        for (var index = 0; index < exhibitions.Count; index++)
        {
            Instantiate(exhibitions[index], origin + new Vector3(index * 100, 0, 0), Quaternion.identity);
        }

        _isPlaced = true;
        
        EventManager.TriggerEvent(EventType.EventExhibitionsPlaced, new EventParam());
    }
}
