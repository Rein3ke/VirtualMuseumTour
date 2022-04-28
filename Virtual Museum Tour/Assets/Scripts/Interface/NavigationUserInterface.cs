using System.Linq;
using Events;
using Player;
using UnityEngine;
using UnityEngine.UI;
using EventType = Events.EventType;

namespace Interface
{
    /// <summary>
    /// Controls the user input for the navigation user interface and its elements.
    /// </summary>
    public class NavigationUserInterface : MonoBehaviour
    {
        #region Serialized Fields

        /// <summary>
        /// Container for the dropdown menu.
        /// </summary>
        [SerializeField] private GameObject sideBar;
        /// <summary>
        /// Container for buttons.
        /// </summary>
        [SerializeField] private GameObject buttonContainer;
        /// <summary>
        /// Dropdown that contains PlayerSpawnPoints to instantiate the player at.
        /// </summary>
        [SerializeField] private Dropdown dropdown;
        /// <summary>
        /// A button to open the doll house view.
        /// </summary>
        [SerializeField] private Button dollHouseViewButton;

        #endregion
        
        #region Members

        /// <summary>
        /// Reference to the doll house view camera. 
        /// </summary>
        private DollHouseView.DollHouseView _dollHouseViewCamera;

        #endregion

        #region Unity Methods

        /// <summary>
        /// Adds the DropdownValueChanged method as a callback for the dropdowns value changed event.
        /// </summary>
        private void Start()
        {
            // fill dropdown with all the spawn points from PlayerSpawnController
            dropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(dropdown); });
        }

        /// <summary>
        /// Subscribes to the PlayerSpawnPointsLoaded event and calls RefreshDropdownContent afterwards.
        /// This is done to ensure that the dropdown is filled with all PlayerSpawnPoints.
        /// Also, adds a Listener to the "open doll house view" button.
        /// </summary>
        private void OnEnable()
        {
            EventManager.StartListening(EventType.EventPlayerSpawnPointsLoaded, RefreshDropdownContent);
            dollHouseViewButton.onClick.AddListener(OpenDollHouseView);
        }

        /// <summary>
        /// Unsubscribes from all events.
        /// </summary>
        private void OnDisable()
        {
            EventManager.StopListening(EventType.EventPlayerSpawnPointsLoaded, RefreshDropdownContent);
            dollHouseViewButton.onClick.RemoveListener(OpenDollHouseView);

            // EventManager.StopListening(EventType.EventSpawnPlayer, SetCameraToActivePlayer);
        }

        #endregion

        #region Interface Handling

        /// <summary>
        /// Sets all UI GameObjects to active.
        /// </summary>
        public void ShowInterface()
        {
            sideBar.SetActive(true); // Activate the side bar that contains the dropdown
            buttonContainer.SetActive(true); 
        }

        /// <summary>
        /// Disables all UI GameObjects.
        /// </summary>
        public void HideInterface()
        {
            sideBar.SetActive(false);
            buttonContainer.SetActive(false);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Enables the doll house view if its reference is already set. Otherwise, instantiates a new one.
        /// </summary>
        private void OpenDollHouseView()
        {
            if (_dollHouseViewCamera) // enable the current doll house view, if there is a reference set in the member variable
            {
                _dollHouseViewCamera.gameObject.SetActive(true);
            }
            else // if there is no reference set, instantiate a DollHouseView prefab instead
            {
                var dollHousePrefab = Instantiate(Resources.Load("Prefabs/DollHouseViewCamera")) as GameObject;

                if (dollHousePrefab == null)
                {
                    Debug.LogWarning("DollHouseView prefab is null and couldn't be initialized!");
                    return;
                }

                _dollHouseViewCamera = dollHousePrefab.GetComponent<DollHouseView.DollHouseView>(); // store the reference
            }
        }

        /// <summary>
        /// Receives an array of PlayerSpawnPoints via the EventParam struct
        /// and fills the dropdown with a list of OptionData objects each containing a PlayerSpawnPoint name.
        /// </summary>
        /// <param name="eventParam">Array of PlayerSpawnPoints.</param>
        private void RefreshDropdownContent(EventParam eventParam)
        {
            if (eventParam.EventPlayerSpawnPoints == null) return; // if no PlayerSpawnPoint[] is passed, return

            var playerSpawnPoints = eventParam.EventPlayerSpawnPoints;
            
            dropdown.options.Clear(); // reset list
            
            if (playerSpawnPoints.Length <= 0) return; // return, if no PlayerSpawnPoints are available

            // transform PlayerSpawnPoints[] into OptionData objects each containing a PlayerSpawnPoint name as text
            // and pass them as a list to the local optionData variable.
            var optionData = playerSpawnPoints
                .Select(spawnPoint => new Dropdown.OptionData {text = spawnPoint.GetComponent<PlayerSpawnPoint>().PlayerSpawnName})
                .ToList();

            dropdown.AddOptions(optionData); // add the list of OptionData objects to the dropdown
        }

        /// <summary>
        /// Gets called when a new option is selected in the dropdown.
        /// Triggers a teleport request.
        /// </summary>
        /// <param name="change">A copy of the dropdown object with the selected option.</param>
        private void DropdownValueChanged(Dropdown change)
        {
            EventManager.TriggerEvent(EventType.EventTeleportRequest, new EventParam
            {
                EventString = change.captionText.text
            });
        }

        #endregion
    }
}