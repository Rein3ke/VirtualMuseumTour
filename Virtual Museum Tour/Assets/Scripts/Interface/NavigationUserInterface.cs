using System;
using System.Linq;
using Events;
using UnityEngine;
using UnityEngine.UI;
using EventType = Events.EventType;

namespace Interface
{
    public class NavigationUserInterface : MonoBehaviour
    {
        // serialize fields
        [SerializeField] private GameObject sideBar;
        [SerializeField] private Dropdown dropdown;

        public void ShowInterface()
        {
            sideBar.SetActive(true);
        }

        public void HideInterface()
        {
            sideBar.SetActive(false);
        }

        #region Unity related methods

        private void Awake()
        {
            GetComponent<Canvas>();
        }

        private void Start()
        {
            // fill dropdown with all the spawn points from PlayerSpawnController
            dropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(dropdown); });

            // HideInterface();
        }

        private void OnEnable()
        {
            EventManager.StartListening(EventType.EventPlayerSpawnPointsLoaded, UpdateDropdown);
            //EventManager.StartListening(EventType.EventSpawnPlayer, SetCameraToActivePlayer);
        }

        private void OnDisable()
        {
            EventManager.StopListening(EventType.EventPlayerSpawnPointsLoaded, UpdateDropdown);
            //EventManager.StopListening(EventType.EventSpawnPlayer, SetCameraToActivePlayer);
        }

        #endregion

        #region Dropdown methods

        private void UpdateDropdown(EventParam eventParam)
        {
            if (eventParam.Param6 == null) return;

            var playerSpawnPoints = eventParam.Param6;
            
            dropdown.options.Clear(); // reset list
            
            if (playerSpawnPoints.Length <= 0) return;

            var optionData = playerSpawnPoints
                .Select(spawnPoint => new Dropdown.OptionData {text = spawnPoint.GetComponent<PlayerSpawnPoint>().PlayerSpawnName})
                .ToList();

            dropdown.AddOptions(optionData);
        }

        private void DropdownValueChanged(Dropdown change)
        {
            var eventParam = new EventParam
            {
                Param1 = change.captionText.text
            };
            EventManager.TriggerEvent(EventType.EventTeleportRequest, eventParam);
        }

        #endregion
    }
}