using System.Collections;
using System.Linq;
using Events;
using UnityEngine;
using EventType = Events.EventType;
using Random = UnityEngine.Random;

namespace Controller
{
    /// <summary>
    /// Manages all PlayerSpawnPoints within the main scene and stores them in an array.
    /// Also manages the instantiation of a player character at a PlayerSpawnPoint.
    /// </summary>
    public class PlayerSpawnController : MonoBehaviour
    {
        #region Constants

        /// <summary>
        /// Path to the prefab of the player in the resources folder.
        /// </summary>
        private const string FirstPersonPlayerPath = "Prefabs/FirstPersonPlayer";

        #endregion

        #region Members

        /// <summary>
        /// Reference to the current player prefab in the scene.
        /// </summary>
        private GameObject _currentPlayer;
        /// <summary>
        /// Player prefab to be instantiated.
        /// </summary>
        private GameObject _playerPrefab;
        /// <summary>
        /// List of all PlayerSpawnPoints in the scene.
        /// </summary>
        private PlayerSpawnPoint[] PlayerSpawnPoints { get; set; }

        #endregion

        #region Unity Methods

        /// <summary>
        /// Loads the player prefab from the resources folder and stores it in the _playerPrefab variable.
        /// If the player prefab is not found, an error is thrown.
        /// </summary>
        private void Awake()
        {
            _playerPrefab = Resources.Load(FirstPersonPlayerPath) as GameObject; // load first person player prefab
            if (_playerPrefab == null) // if player prefab is not found, throw error
            {
                Debug.LogError("Couldn't load player prefab! Please make sure the path is correct.");
            }
        }

        /// <summary>
        /// Subscribes to the EventTeleportRequested event, as well as the EventExhibitionsPlaced event.
        /// EventTeleportRequest is used to teleport the player to a specific PlayerSpawnPoint.
        /// EventExhibitionsPlaced is used to update the list of PlayerSpawnPoints in the scene.
        /// </summary>
        private void OnEnable()
        {
            EventManager.StartListening(EventType.EventTeleportRequest, SetTeleportRequestOn);
            EventManager.StartListening(EventType.EventExhibitionsPlaced, OnExhibitionsPlaced);
        }

        /// <summary>
        /// Unsubscribes from all events.
        /// </summary>
        private void OnDisable()
        {
            EventManager.StopListening(EventType.EventTeleportRequest, SetTeleportRequestOn);
            EventManager.StopListening(EventType.EventExhibitionsPlaced, OnExhibitionsPlaced);
        }

        #endregion

        #region Player Instantiation

        /// <summary>
        /// Coroutine to instantiate a new player prefab at a specific position.
        /// </summary>
        /// <param name="playerPrefab">Player prefab.</param>
        /// <param name="position">Spawn position.</param>
        private IEnumerator SpawnPlayer(GameObject playerPrefab, Vector3 position)
        {
            // reset current player reference & destroy previous player game object
            if (_currentPlayer != null)
            {
                Destroy(_currentPlayer);
                _currentPlayer = null;
            }
            
            // wait for one frame to finish, so that the player prefab is not instantiated before the player is destroyed
            // otherwise the new player instance will be destroyed at the end of the frame
            yield return null; 
            
            _currentPlayer = Instantiate(playerPrefab, position, Quaternion.identity); // instantiate player prefab at the desired position
            if (_currentPlayer != null) // if the player prefab was successfully instantiated, trigger the EventSpawnPlayer event
            {
                Debug.Log($"Spawn player {playerPrefab.name} at position: X:{position.x}, Y:{position.y}, Z:{position.z}.");

                EventManager.TriggerEvent(EventType.EventSpawnPlayer, new EventParam // inform other listeners that the player was successfully spawned
                {
                    EventBoolean = true
                });
            }
        }

        /// <summary>
        /// Starts the SpawnPlayer coroutine with the stored player prefab and a random spawn position.
        /// </summary>
        private void SpawnPlayerOnRandomPosition()
        {
            StartCoroutine(SpawnPlayer(_playerPrefab, GetRandomPlayerSpawnPoint().transform.position));
        }

        #endregion

        #region PlayerSpawnPoint

        /// <summary>
        /// Finds all PlayerSpawnPoint game objects in the scene and stores them in a list.
        /// </summary>
        private void UpdatePlayerSpawnPointList()
        {
            PlayerSpawnPoints = FindObjectsOfType<PlayerSpawnPoint>(); // load all player spawn points in current scene
            
            EventManager.TriggerEvent(EventType.EventPlayerSpawnPointsLoaded, new EventParam // inform other listeners that the PlayerSpawnPoints list was updated
            {
                EventPlayerSpawnPoints = PlayerSpawnPoints
            });
        }

        /// <summary>
        /// Receives the name of a PlayerSpawnPoint and returns the first entry from the list of PlayerSpawnPoints that matches the name.
        /// </summary>
        /// <param name="playerSpawnPointName">Name of the PlayerSpawnPoint in the scene.</param>
        /// <returns>PlayerSpawnPoint that matches the name.</returns>
        private PlayerSpawnPoint GetPlayerSpawnPoint(string playerSpawnPointName)
        {
            UpdatePlayerSpawnPointList(); // Update the list of PlayerSpawnPoints in the scene
            
            // find the first PlayerSpawnPoint in the list that matches the name or return null if no match was found
            return PlayerSpawnPoints.FirstOrDefault(playerSpawnPoint =>
                playerSpawnPointName.Equals(playerSpawnPoint.PlayerSpawnName));
        }

        /// <summary>
        /// Returns a random PlayerSpawnPoint from the list of PlayerSpawnPoints.
        /// </summary>
        /// <returns>Random PlayerSpawnPoint.</returns>
        private PlayerSpawnPoint GetRandomPlayerSpawnPoint()
        {
            UpdatePlayerSpawnPointList(); // Update the list of PlayerSpawnPoints in the scene

            var random = Random.Range(0, PlayerSpawnPoints.Length); // get a random index from the list of PlayerSpawnPoints
            var point = PlayerSpawnPoints[random]; // get the PlayerSpawnPoint at the random index
            
            return point; // return the PlayerSpawnPoint
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Calls the SpawnPlayerOnRandomPosition method, if the EventExhibitionsPlaced event is triggered.
        /// That insures that the player is spawned after the main scene is ready.
        /// </summary>
        /// <param name="eventParam">(Obsolete).</param>
        private void OnExhibitionsPlaced(EventParam eventParam)
        {
            SpawnPlayerOnRandomPosition();
        }

        /// <summary>
        /// Receives a PlayerSpawnPoint name or PlayerSpawnPoint object via the eventParam and starts the SpawnPlayer coroutine.
        /// (This method is called by the DollHouseView and the NavigationUserInterface which both uses other parameters.)
        /// </summary>
        /// <param name="eventParam">PlayerSpawnPoint (via eventParam.EventString or via eventParam.EventPlayerSpawnPoint).</param>
        private void SetTeleportRequestOn(EventParam eventParam)
        {
            PlayerSpawnPoint playerSpawnPoint = null;
            if (!string.IsNullOrEmpty(eventParam.EventString)) // if the PlayerSpawnPoint name isn't null or empty, get the PlayerSpawnPoint from the list
            {
                playerSpawnPoint = GetPlayerSpawnPoint(eventParam.EventString);
                Debug.Log($"SetTeleportRequestOn: {eventParam.EventString} (String)");
            } else if (eventParam.EventPlayerSpawnPoint != null) // if the PlayerSpawnPoint name is null or empty, set the PlayerSpawnPoint directly from the eventParam
            {
                playerSpawnPoint = eventParam.EventPlayerSpawnPoint;
                Debug.Log($"SetTeleportRequestOn: {eventParam.EventPlayerSpawnPoint.PlayerSpawnName} (PlayerSpawnPoint)");
            }

            if (playerSpawnPoint == null) // If the PlayerSpawnPoint is still null, throw an error and return
            {
                Debug.LogError("SetTeleportRequestOn [Error]: Player spawn point is null!");
                return;
            }

            StartCoroutine(SpawnPlayer(_playerPrefab, playerSpawnPoint.gameObject.transform.position)); // Start the SpawnPlayer coroutine with the PlayerSpawnPoint position
        }

        #endregion
    }
}