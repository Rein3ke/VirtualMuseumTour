using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigationUserInterface : MonoBehaviour
{
    [SerializeField] private GameObject sideBar;
    [SerializeField] private Dropdown dropdown; 
    
    private GameObject[] _playerSpawnPoints;

    private void Start()
    {
        RefreshPlayerSpawnPointListFromCurrentScene();

        if (_playerSpawnPoints.Length > 0)
        {
            var optionDatas = new List<Dropdown.OptionData>();
            foreach (var playerSpawnPointGameObject in _playerSpawnPoints)
            {
                var optionData = new Dropdown.OptionData
                {
                    text = playerSpawnPointGameObject.GetComponent<PlayerSpawnPoint>().PlayerSpawnName
                };

                optionDatas.Add(optionData);
            }

            dropdown.AddOptions(optionDatas);
        }
        
        dropdown.onValueChanged.AddListener(delegate
        {
            DropdownValueChanged(dropdown);
        });
        //SetActive(sideBar, false);
    }

    private void DropdownValueChanged(Dropdown change)
    {
        PlayerSpawnController.Instance.SetTeleportRequestOn(change.captionText.text);
    }

    private void RefreshPlayerSpawnPointListFromCurrentScene()
    {
        _playerSpawnPoints = GameObject.FindGameObjectsWithTag("PlayerSpawner");
        Debug.Log($"Player spawn points found: {_playerSpawnPoints.Length}");
    }

    private void ToggleUiElement(GameObject uiElement)
    {
        uiElement.SetActive(!uiElement.activeSelf);
    }

    private void SetActive(GameObject uiElement, bool shouldBeActive)
    {
        uiElement.SetActive(shouldBeActive);
    }
}
