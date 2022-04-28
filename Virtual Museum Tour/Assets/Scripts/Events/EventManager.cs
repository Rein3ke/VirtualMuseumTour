using System;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    /// <summary>
    /// Used for communication between classes. Requires an EventManager component on a GameObject in the current scene.
    /// The manager stores events and their listeners and also provides public methods for adding and removing listeners.
    /// The public TriggerEvent method is used to trigger all listeners of an event.
    /// The static class EventType contains multiple event types that can be used.
    /// (This class is based on a post by user "programmer" on StackOverflow:
    /// https://stackoverflow.com/questions/42034245/unity-eventmanager-with-delegate-instead-of-unityevent/42034899#42034899)
    /// </summary>
    public class EventManager : MonoBehaviour
    {
        #region Members

        /// <summary>
        /// Stores a string as key and a list of listeners as value.
        /// </summary>
        private Dictionary<string, Action<EventParam>> eventDictionary;

        #endregion

        #region Static Members

        /// <summary>
        /// Current instance of the EventManager.
        /// </summary>
        private static EventManager _eventManager;

        #endregion

        #region Properties

        /// <summary>
        /// Returns the current instance of the EventManager (only used in this class).
        /// If there is no instance yet, it will be created by using the Init() method.
        /// </summary>
        private static EventManager Instance
        {
            get
            {
                if (_eventManager) return _eventManager; // If there is already an instance, return it.
                
                _eventManager = FindObjectOfType<EventManager>(); // If there is no instance yet, find one in the current scene.

                if (!_eventManager) // If there is no EventManager instance found, throw a warning.
                {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in the scene.");
                }
                else // Initialize the instance found via FindObjectOfType.
                {
                    _eventManager.Init();
                }

                return _eventManager; // Return the initialized instance.
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Creates a dictionary at initialization if it does not already exist to store the events.
        /// </summary>
        private void Init()
        {
            eventDictionary ??= new Dictionary<string, Action<EventParam>>();
        }

        #endregion

        #region Event Management

        /// <summary>
        /// Receives a string as key (event name) and a delegate as value (listener). Then adds the listener to the dictionary.
        /// If the event already exists, the listener is added to the list of listeners.
        /// If the event doesn't exist, a new event is created with the given listener.
        /// </summary>
        /// <param name="eventName">Event name.</param>
        /// <param name="listener">Callback method.</param>
        public static void StartListening(string eventName, Action<EventParam> listener)
        {
            // If the event exists add another listener to the already existing event
            if (Instance.eventDictionary.TryGetValue(eventName, out var thisEvent))
            {
                thisEvent += listener; // Add listener to the existing event
                
                Instance.eventDictionary[eventName] = thisEvent; // Update the Dictionary
            }
            else // If the event doesn't exist create a new one and add the given listener
            {
                thisEvent = listener;
                Instance.eventDictionary.Add(eventName, thisEvent);
            }
        }

        /// <summary>
        /// Receives a string as key (event name) and a delegate as value (listener). Then removes the listener from the dictionary if it exists.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="listener"></param>
        public static void StopListening(string eventName, Action<EventParam> listener)
        {
            if (_eventManager == null) return;

            if (!Instance.eventDictionary.TryGetValue(eventName, out var thisEvent)) return; // Return if the event doesn't exist
            
            thisEvent -= listener; // If found, remove the given listener from the existing event
                
            Instance.eventDictionary[eventName] = thisEvent; // Update the Dictionary
        }

        /// <summary>
        /// Triggers an event with the given event name and event parameter.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="eventParam"></param>
        public static void TriggerEvent(string eventName, EventParam eventParam)
        {
            if (Instance.eventDictionary.TryGetValue(eventName, out var thisEvent))
            {
                thisEvent?.Invoke(eventParam);
            }
        }

        #endregion
    }
}