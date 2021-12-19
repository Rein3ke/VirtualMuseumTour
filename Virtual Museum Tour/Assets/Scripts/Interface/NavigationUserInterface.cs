using System.Linq;
using Controller;
using UnityEngine;
using UnityEngine.UI;

namespace Interface
{
    public class NavigationUserInterface : MonoBehaviour
    {
        [SerializeField] private GameObject sideBar;
        [SerializeField] private Dropdown dropdown;

        private void Start()
        {
            // fill dropdown with all the spawn points from PlayerSpawnController
            DropdownSetup();

            void DropdownSetup()
            {
                FillDropdown();
                dropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(dropdown); });
            }
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
        }

        public void HideInterface()
        {
            sideBar.SetActive(false);
        }
    }
}