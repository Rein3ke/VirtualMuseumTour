using Controller;
using Player;
using UnityEngine;

namespace Events
{
    /// <summary>
    /// Structure that can hold data for an event and is passed as a parameter to the event.
    /// To set theses values is optional but is required for some events.
    /// </summary>
    public struct EventParam
    {
        /// <summary>
        /// String.
        /// </summary>
        public string EventString;
        /// <summary>
        /// Integer.
        /// </summary>
        public int EventInteger;
        /// <summary>
        /// Float.
        /// </summary>
        public float EventFloat;
        /// <summary>
        /// Boolean.
        /// </summary>
        public bool EventBoolean;
        /// <summary>
        /// ApplicationState.
        /// </summary>
        public ApplicationState EventApplicationState;
        /// <summary>
        /// List of PlayerSpawnPoints.
        /// </summary>
        public PlayerSpawnPoint[] EventPlayerSpawnPoints;
        /// <summary>
        /// PlayerSpawnPoint.
        /// </summary>
        public PlayerSpawnPoint EventPlayerSpawnPoint;
        /// <summary>
        /// Exhibit object.
        /// </summary>
        public Exhibit EventExhibit;
        /// <summary>
        /// AudioClip.
        /// </summary>
        public AudioClip EventAudioClip;
    }
}