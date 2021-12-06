using System;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private string playerSpawnName;
    
    private GameObject _currentPlayer;

    private void Start()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        _currentPlayer = Instantiate(player, transform.position, Quaternion.identity);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Destroy(_currentPlayer.gameObject);
            
            _currentPlayer = null;
            
            SpawnPlayer();
        }
    }
}
