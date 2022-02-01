using System.Collections;
using System.Linq;
using UnityEngine;

namespace Controller
{
    public class PlayerSpawnController : MonoBehaviour
    {
        private const string FirstPersonPlayerPath = "Prefabs/FirstPersonPlayer";
        public static PlayerSpawnController Instance { get; private set; }

        [SerializeField] private bool spawnPlayerOnStart;

        public PlayerSpawnPoint[] PlayerSpawnPoints { get; private set; }

        private GameObject _currentPlayer;
        private GameObject _playerPrefab;

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
            // load all player spawn points in current scene
            PlayerSpawnPoints = FindObjectsOfType<PlayerSpawnPoint>();

            // load first person player prefab
            _playerPrefab = Resources.Load(FirstPersonPlayerPath) as GameObject;
            if (_playerPrefab == null)
            {
                Debug.LogError("Couldn't load player prefab!");
                return;
            }

            // If true, instantiate player prefab on position x
            if (spawnPlayerOnStart)
            {
                StartCoroutine(InstantiatePlayerPrefabOnPosition(_playerPrefab, PlayerSpawnPoints[0].gameObject.transform.position));
            }
        }

        /// <summary>
        /// Coroutine to instantiate a new player after waiting one frame.
        /// </summary>
        /// <param name="playerPrefab">Player gameObject or prefab.</param>
        /// <param name="position">Spawn position.</param>
        private IEnumerator InstantiatePlayerPrefabOnPosition(GameObject playerPrefab, Vector3 position)
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
            }
        }

        public void SetTeleportRequestOn(string playerSpawnPointName)
        {
            if (string.IsNullOrEmpty(playerSpawnPointName))
            {
                Debug.LogError("SetTeleportRequestOn [Error]: Parameter is empty or null!");
                return;
            }

            var playerSpawnPoint = GetPlayerSpawnPointByName(playerSpawnPointName);
            if (playerSpawnPoint == null)
            {
                Debug.LogError("SetTeleportRequestOn [Error]: Player spawn point is null!");
                return;
            }

            StartCoroutine(InstantiatePlayerPrefabOnPosition(_playerPrefab, playerSpawnPoint.gameObject.transform.position));
        }

        private PlayerSpawnPoint GetPlayerSpawnPointByName(string playerSpawnPointName)
        {
            return PlayerSpawnPoints.FirstOrDefault(playerSpawnPoint =>
                playerSpawnPointName.Equals(playerSpawnPoint.PlayerSpawnName));
        }

        private void OnDestroy()
        {
            // reset instance
            if (Instance != null && Instance == this) Instance = null;
        }
    }
}