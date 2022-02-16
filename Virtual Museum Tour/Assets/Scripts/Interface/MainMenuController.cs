using Controller;
using Events;
using UnityEngine;
using UnityEngine.UI;
using EventType = Events.EventType;

namespace Interface
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button startButton;

        private void OnEnable()
        {
            startButton.onClick.AddListener(StartButtonEvent);
        }

        private void StartButtonEvent()
        {
            var eventParam = new EventParam
            {
                Param1 = "Scenes/Exhibitions/Ladestrasse",
                Param4 = false
            };
            EventManager.TriggerEvent(EventType.EventLoadScene, eventParam);

            var eventParam2 = new EventParam
            {
                Param5 = ApplicationState.Main
            };
            EventManager.TriggerEvent(EventType.EventSetState, eventParam2);
        }

        private void OnDisable()
        {
            startButton.onClick.RemoveListener(StartButtonEvent);
        }
    }
}
