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
        [SerializeField] private GameObject buttonContainer;
        [SerializeField] private Dropdown dropdown;
        [SerializeField] private Button dollHouseViewButton;
        
        private DollHouseView.DollHouseView _dollHouseViewCamera;

        public void ShowInterface()
        {
            sideBar.SetActive(true);
            buttonContainer.SetActive(true);
        }

        public void HideInterface()
        {
            sideBar.SetActive(false);
            buttonContainer.SetActive(false);
        }

        private void OnDollHouseViewButtonClick()
        {
            if (_dollHouseViewCamera)
            {
                _dollHouseViewCamera.gameObject.SetActive(true);
            }
            else
            {
                var dollHousePrefab = Instantiate(Resources.Load("Prefabs/DollHouseViewCamera")) as GameObject;

                if (dollHousePrefab == null)
                {
                    Debug.LogWarning("DollHouseView is null and couldn't be initialized!");
                    return;
                }

                _dollHouseViewCamera = dollHousePrefab.GetComponent<DollHouseView.DollHouseView>();
            }
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
        }

        private void OnEnable()
        {
            EventManager.StartListening(EventType.EventPlayerSpawnPointsLoaded, UpdateDropdown);
            dollHouseViewButton.onClick.AddListener(OnDollHouseViewButtonClick);

            // EventManager.StartListening(EventType.EventSpawnPlayer, SetCameraToActivePlayer);
        }

        private void OnDisable()
        {
            EventManager.StopListening(EventType.EventPlayerSpawnPointsLoaded, UpdateDropdown);
            dollHouseViewButton.onClick.RemoveListener(OnDollHouseViewButtonClick);

            // EventManager.StopListening(EventType.EventSpawnPlayer, SetCameraToActivePlayer);
        }

        #endregion

        #region Dropdown methods

        private void UpdateDropdown(EventParam eventParam)
        {
            if (eventParam.EventPlayerSpawnPoints == null) return;

            var playerSpawnPoints = eventParam.EventPlayerSpawnPoints;
            
            dropdown.options.Clear(); // reset list
            
            if (playerSpawnPoints.Length <= 0) return;

            var optionData = playerSpawnPoints
                .Select(spawnPoint => new Dropdown.OptionData {text = spawnPoint.GetComponent<PlayerSpawnPoint>().PlayerSpawnName})
                .ToList();

            dropdown.AddOptions(optionData);
        }

        private void DropdownValueChanged(Dropdown change)
        {
            EventManager.TriggerEvent(EventType.EventTeleportRequest, new EventParam()
            {
                EventString = change.captionText.text
            });
        }

        #endregion
    }
}