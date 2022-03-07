using System.Collections;
using System.Linq;
using Events;
using UnityEngine;
using EventType = Events.EventType;
using Random = UnityEngine.Random;

namespace Controller
{
    public class PlayerSpawnController : MonoBehaviour
    {
        private const string FirstPersonPlayerPath = "Prefabs/FirstPersonPlayer";

        private GameObject _currentPlayer;
        private GameObject _playerPrefab;

        private PlayerSpawnPoint[] PlayerSpawnPoints { get; set; }

        private void Awake()
        {
            // load first person player prefab
            _playerPrefab = Resources.Load(FirstPersonPlayerPath) as GameObject;
            if (_playerPrefab == null)
            {
                Debug.LogError("Couldn't load player prefab!");
            }
        }

        private void OnEnable()
        {
            EventManager.StartListening(EventType.EventTeleportRequest, SetTeleportRequestOn);
            EventManager.StartListening(EventType.EventExhibitionsPlaced, OnExhibitionsPlaced);
        }

        private void OnDisable()
        {
            EventManager.StopListening(EventType.EventTeleportRequest, SetTeleportRequestOn);
            EventManager.StopListening(EventType.EventExhibitionsPlaced, OnExhibitionsPlaced);
        }

        private void OnExhibitionsPlaced(EventParam eventParam)
        {
            SpawnPlayerOnRandomPosition();
        }

        private void UpdatePlayerSpawnPointList()
        {
            // load all player spawn points in current scene
            PlayerSpawnPoints = FindObjectsOfType<PlayerSpawnPoint>();
            var eventParam = new EventParam
            {
                EventPlayerSpawnPoints = PlayerSpawnPoints
            };
            EventManager.TriggerEvent(EventType.EventPlayerSpawnPointsLoaded, eventParam);
        }

        /// <summary>
        /// Coroutine to instantiate a new player after waiting one frame.
        /// </summary>
        /// <param name="playerPrefab">Player gameObject or prefab.</param>
        /// <param name="position">Spawn position.</param>
        private IEnumerator SpawnPlayer(GameObject playerPrefab, Vector3 position)
        {
            // reset current player reference & destroy previous player game object
            if (_currentPlayer != null)
            {
                Destroy(_currentPlayer);
                _currentPlayer = null;
            }

            yield return null; // wait for one frame to finish
            
            _currentPlayer = Instantiate(playerPrefab, position, Quaternion.identity);
            if (_currentPlayer != null)
            {
                Debug.Log($"Spawn player {playerPrefab.name} at position: X:{position.x}, Y:{position.y}, Z:{position.z}.");

                var eventParam = new EventParam
                {
                    EventBoolean = true
                };
                EventManager.TriggerEvent(EventType.EventSpawnPlayer, eventParam);
            }
        }

        private void SetTeleportRequestOn(EventParam eventParam)
        {
            if (string.IsNullOrEmpty(eventParam.EventString))
            {
                Debug.LogError("SetTeleportRequestOn [Error]: Parameter is empty or null!");
                return;
            }

            var playerSpawnPoint = GetPlayerSpawnPoint(eventParam.EventString);
            if (playerSpawnPoint == null)
            {
                Debug.LogError("SetTeleportRequestOn [Error]: Player spawn point is null!");
                return;
            }

            StartCoroutine(SpawnPlayer(_playerPrefab, playerSpawnPoint.gameObject.transform.position));
        }

        private void SpawnPlayerOnRandomPosition()
        {
            StartCoroutine(SpawnPlayer(_playerPrefab, GetRandomPlayerSpawnPoint().transform.position));
        }

        private PlayerSpawnPoint GetPlayerSpawnPoint(string playerSpawnPointName)
        {
            UpdatePlayerSpawnPointList();
            
            return PlayerSpawnPoints.FirstOrDefault(playerSpawnPoint =>
                playerSpawnPointName.Equals(playerSpawnPoint.PlayerSpawnName));
        }

        private PlayerSpawnPoint GetRandomPlayerSpawnPoint()
        {
            UpdatePlayerSpawnPointList();
            
            Debug.Log($"GetRandomPlayerSpawnPoint: Spawn Points Length: {PlayerSpawnPoints.Length}");
            
            var random = Random.Range(0, PlayerSpawnPoints.Length);
            var point = PlayerSpawnPoints[random];
            
            return point;
        }
    }
}