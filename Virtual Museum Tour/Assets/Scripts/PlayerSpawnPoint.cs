using System;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    [SerializeField] private string playerSpawnName;

    public string PlayerSpawnName => playerSpawnName;
}
