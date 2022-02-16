using System;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public class EventManager : MonoBehaviour
    {
        private Dictionary<string, Action<EventParam>> eventDictionary;

        private static EventManager _eventManager;

        public static EventManager Instance
        {
            get
            {
                if (_eventManager) return _eventManager;
                
                _eventManager = FindObjectOfType<EventManager>();

                if (!_eventManager)
                {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    _eventManager.Init();
                }

                return _eventManager;
            }
        }

        private void Init()
        {
            eventDictionary ??= new Dictionary<string, Action<EventParam>>();
        }

        public static void StartListening(string eventName, Action<EventParam> listener)
        {
            if (Instance.eventDictionary.TryGetValue(eventName, out var thisEvent))
            {
                // Add more event to the existing one
                thisEvent += listener;
                
                // Update the Dictionary
                Instance.eventDictionary[eventName] = thisEvent;
            }
            else
            {
                // Add event to the Dictionary for the first time
                thisEvent = listener;
                Instance.eventDictionary.Add(eventName, thisEvent);
            }
        }

        public static void StopListening(string eventName, Action<EventParam> listener)
        {
            if (_eventManager == null) return;

            if (Instance.eventDictionary.TryGetValue(eventName, out var thisEvent))
            {
                // Remove event from the existing one
                thisEvent -= listener;
                
                // Update the Dictionary
                Instance.eventDictionary[eventName] = thisEvent;
            }
        }

        public static void TriggerEvent(string eventName, EventParam eventParam)
        {
            if (Instance.eventDictionary.TryGetValue(eventName, out var thisEvent))
            {
                thisEvent?.Invoke(eventParam);
            }
        }
    }
}