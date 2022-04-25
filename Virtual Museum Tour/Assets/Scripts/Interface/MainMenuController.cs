using Controller;
using Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EventType = Events.EventType;

namespace Interface
{
    /// <summary>
    /// Manages the main menu and its UI elements within the MainMenu scene.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        #region Serialized Fields

        /// <summary>
        /// TextMeshProUGUI component that stores a introduction text or welcome text.
        /// </summary>
        [Header("Text Elements")]
        [SerializeField] private TextMeshProUGUI welcomeText;
        /// <summary>
        /// String, that stores a introduction text or welcome text.
        /// </summary>
        [SerializeField][TextArea] private string welcomeTextContent;
        
        [Header("Buttons")]
        [SerializeField] private Button startButton;

        #endregion

        #region Unity Methods

        /// <summary>
        /// Sets the text of the TextMeshProUGUI component to the welcomeTextContent string.
        /// </summary>
        private void Start()
        {
            welcomeText.text = welcomeTextContent;
        }

        /// <summary>
        /// Adds a listener to the startButton's onClick event.
        /// </summary>
        private void OnEnable()
        {
            startButton.onClick.AddListener(StartButtonEvent);
        }

        /// <summary>
        /// Removes a listener from the startButton's onClick event.
        /// </summary>
        private void OnDisable()
        {
            startButton.onClick.RemoveListener(StartButtonEvent);
        }

        #endregion

        /// <summary>
        /// Triggers the EventLoadScene event and the EventSetState event on button click.
        /// </summary>
        private void StartButtonEvent()
        {
            EventManager.TriggerEvent(EventType.EventLoadScene, new EventParam // trigger an event to load the main scene non-additively
            {
                EventString = "Scenes/Exhibitions/ExhibitionMainScene",
                EventBoolean = false
            });
            
            EventManager.TriggerEvent(EventType.EventSetState, new EventParam // trigger an event to set the application state to MAIN
            {
                EventApplicationState = ApplicationState.Main
            });
        }
    }
}
