using UnityEngine;

namespace Controller
{
    public class PlayerSpawnController : MonoBehaviour
    {
        public static PlayerSpawnController Instance { get; private set; }
    
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private bool spawnPlayerOnStart;

        private PlayerSpawnPoint[] _playerSpawnPoints;
        private GameObject _currentPlayer;

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
            _playerSpawnPoints = FindObjectsOfType<PlayerSpawnPoint>();
        
            // If true, instantiate player prefab on position x
            if (spawnPlayerOnStart)
            {
                InstantiatePlayerPrefabOnPosition(_playerSpawnPoints[0].gameObject.transform.position);
            }
        }

        private void InstantiatePlayerPrefabOnPosition(Vector3 position)
        {
            if (_currentPlayer != null) Destroy(_currentPlayer);
        
            _currentPlayer = Instantiate(playerPrefab, position, Quaternion.identity);
        }

        public bool SetTeleportRequestOn(string playerSpawnPointTitle)
        {
            if (string.IsNullOrEmpty(playerSpawnPointTitle))
            {
                Debug.LogError("SetTeleportRequestOn [Error]: Parameter is empty or null!");
                return false;
            }
        
            foreach (PlayerSpawnPoint playerSpawnPoint in _playerSpawnPoints)
            {
                if (playerSpawnPoint.PlayerSpawnName.Equals(playerSpawnPointTitle))
                {
                    InstantiatePlayerPrefabOnPosition(playerSpawnPoint.gameObject.transform.position);
                    return true;
                }
            }

            return false;
        }
    }
}
