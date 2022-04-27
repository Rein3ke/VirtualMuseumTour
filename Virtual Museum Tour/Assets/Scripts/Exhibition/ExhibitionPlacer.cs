using System.Collections.Generic;
using Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using EventType = Events.EventType;

/// <summary>
/// The ExhibitionPlacer is a script that is used to place all Exhibition Prefabs in the scene that are set in the Inspector.
/// </summary>
public class ExhibitionPlacer : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField] private GameObject[] exhibitionList;
    [SerializeField] private Vector3 origin;

    #endregion

    #region Members

    private bool _isPlaced;

    #endregion

    #region Unity Methods

    /// <summary>
    /// Throws a warning if no Exhibition Prefabs are set in the Inspector.
    /// </summary>
    private void Start()
    {
        if (exhibitionList.Length == 0)
        {
            Debug.LogWarning("Caution. The exhibition list is empty. Remember to fill the list in the inspector with at least one exhibition prefab before starting the application.");
        }
    }

    /// <summary>
    /// Subscribes to the SceneManager.sceneLoaded event to trigger the placement of the exhibitions.
    /// </summary>
    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneManager_OnSceneLoaded;
    }

    /// <summary>
    /// Unsubscribes from the SceneManager.sceneLoaded event.
    /// </summary>
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneManager_OnSceneLoaded;
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Calls PlaceExhibitionsInGrid() when the ExhibitionMainScene is loaded.
    /// </summary>
    /// <param name="arg0">Current scene (Unused).</param>
    /// <param name="arg1">LoadSceneMode (Unused).</param>
    private void SceneManager_OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("ExhibitionMainScene"))
        {
            PlaceExhibitionsInGrid(exhibitionList);
        }
    }

    #endregion

    /// <summary>
    /// Instantiates all exhibition prefabs in a 1xn grid layout.
    /// If the exhibition list is empty, the method throws an error and returns.
    /// </summary>
    /// <param name="exhibitions">A list of exhibition prefabs.</param>
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
            Instantiate(exhibitions[index], origin + new Vector3(index * 100, 0, 0), Quaternion.identity); // Instantiate a exhibition prefab at the origin + the index * 100.
        }

        _isPlaced = true;
        
        EventManager.TriggerEvent(EventType.EventExhibitionsPlaced, new EventParam()); // Trigger an event to inform other systems that the exhibitions are placed.
    }
}
