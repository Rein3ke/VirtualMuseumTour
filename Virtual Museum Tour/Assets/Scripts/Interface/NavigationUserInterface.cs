using System;
using System.Linq;
using Controller;
using UnityEngine;
using UnityEngine.UI;

namespace Interface
{
    public class NavigationUserInterface : MonoBehaviour, IUserInterface
    {
        public static NavigationUserInterface Instance { get; private set; }
        
        [SerializeField] private GameObject sideBar;
        [SerializeField] private Dropdown dropdown;
        private bool _isVisible;
        public event EventHandler<OnVisibilityChangeEventArgs> OnVisibilityChange;

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                InvokeOnVisibilityChange(_isVisible);
            }
        }

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
            // 1. fill dropdown with all the spawn points from PlayerSpawnController
            DropdownSetup();

            void DropdownSetup()
            {
                FillDropdown();
                dropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(dropdown); });
            }

            // 2. subscribe from events
            SelectionManager.Instance.OnExhibitSelected += OnExhibitSelected;
        }

        private void FillDropdown()
        {
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

        #region Events

        public void InvokeOnVisibilityChange(bool isVisible)
        {
            OnVisibilityChange?.Invoke(this, new OnVisibilityChangeEventArgs(isVisible));
        }

        #endregion

        #region Event Handling

        private void OnExhibitSelected(object sender, EventArgs e)
        {
            HideInterface();
        }

        #endregion

        private void OnDestroy()
        {
            // unsubscribe from events
            SelectionManager.Instance.OnExhibitSelected -= OnExhibitSelected;
            // reset instance
            if (Instance != null && Instance == this)
            {
                Instance = null;
            }
        }
    }
}