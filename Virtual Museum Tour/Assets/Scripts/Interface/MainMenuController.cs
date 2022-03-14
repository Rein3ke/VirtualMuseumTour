using System;
using Controller;
using Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EventType = Events.EventType;

namespace Interface
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Text Elements")]
        [SerializeField] private TextMeshProUGUI welcomeText;
        [SerializeField][TextArea] private string welcomeTextContent;
        
        [Header("Buttons")]
        [SerializeField] private Button startButton;

        private void OnEnable()
        {
            startButton.onClick.AddListener(StartButtonEvent);
        }

        private void OnDisable()
        {
            startButton.onClick.RemoveListener(StartButtonEvent);
        }

        private void Start()
        {
            welcomeText.text = welcomeTextContent;
        }

        private void StartButtonEvent()
        {
            var eventParam = new EventParam
            {
                EventString = "Scenes/Exhibitions/ExhibitionMainScene",
                EventBoolean = false
            };
            EventManager.TriggerEvent(EventType.EventLoadScene, eventParam);

            var eventParam2 = new EventParam
            {
                EventApplicationState = ApplicationState.Main
            };
            EventManager.TriggerEvent(EventType.EventSetState, eventParam2);
        }
    }
}
