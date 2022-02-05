using System;
using System.Linq;
using Controller;
using UnityEngine;
using UnityEngine.UI;

namespace Interface
{
    public class NavigationUserInterface : MonoBehaviour, IUserInterface
    {
        // static members
        public static NavigationUserInterface Instance { get; private set; }
        
        // serialize fields
        [SerializeField] private GameObject sideBar;
        [SerializeField] private Dropdown dropdown;
        
        // members
        private bool _isVisible;
        
        // events
        public event EventHandler<OnVisibilityChangeEventArgs> OnVisibilityChange;

        // properties
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                InvokeOnVisibilityChange(_isVisible);
            }
        }

        #region Unity related methods

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
            // fill dropdown with all the spawn points from PlayerSpawnController
            UpdateDropdown();
            dropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(dropdown); });

            SelectionManager.Instance.OnExhibitSelected += SelectionManager_OnExhibitSelected; // subscribe from events
            ExhibitDetailsUserInterface.Instance.OnVisibilityChange += ExhibitDetailsUserInterface_OnVisibilityChange;
            PlayerSpawnController.Instance.OnPlayerSpawnPointsListUpdated += PlayerSpawnController_OnPlayerSpawnPointsListUpdated;

            ShowInterface();
        }

        private void OnDestroy()
        {
            // reset instance
            if (Instance != null && Instance == this)
            {
                Instance = null;
            }
        }

        private void OnDisable()
        {
            // unsubscribe from events
            if (SelectionManager.Instance != null)
                SelectionManager.Instance.OnExhibitSelected -= SelectionManager_OnExhibitSelected;
            
            if (ExhibitDetailsUserInterface.Instance != null)
                ExhibitDetailsUserInterface.Instance.OnVisibilityChange -= ExhibitDetailsUserInterface_OnVisibilityChange;
        }

        #endregion

        public void ShowInterface()
        {
            sideBar.SetActive(true);
            IsVisible = true;
        }

        public void HideInterface()
        {
            sideBar.SetActive(false);
            IsVisible = false;
        }

        #region Dropdown methods

        private void UpdateDropdown()
        {
            dropdown.options.Clear(); // reset list
            
            var playerSpawnPoints = PlayerSpawnController.Instance.PlayerSpawnPoints;
            if (playerSpawnPoints.Length <= 0) return;

            var optionData = playerSpawnPoints
                .Select(spawnPoint => new Dropdown.OptionData {text = spawnPoint.GetComponent<PlayerSpawnPoint>().PlayerSpawnName})
                .ToList();

            dropdown.AddOptions(optionData);
        }

        private void DropdownValueChanged(Dropdown change)
        {
            PlayerSpawnController.Instance.SetTeleportRequestOn(change.captionText.text);
        }

        #endregion

        #region Events

        public void InvokeOnVisibilityChange(bool isVisible)
        {
            OnVisibilityChange?.Invoke(this, new OnVisibilityChangeEventArgs(isVisible));
        }

        #endregion

        #region Event Handling

        private void SelectionManager_OnExhibitSelected(object sender, EventArgs e)
        {
            HideInterface();
        }

        private void ExhibitDetailsUserInterface_OnVisibilityChange(object sender, OnVisibilityChangeEventArgs e)
        {
            if (!e.IsVisible)
            {
                ShowInterface();
            }
        }
        
        private void PlayerSpawnController_OnPlayerSpawnPointsListUpdated(object sender, EventArgs e)
        {
            UpdateDropdown();
        }

        #endregion
    }
}