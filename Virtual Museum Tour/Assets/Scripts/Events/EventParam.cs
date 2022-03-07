using Controller;
using UnityEngine;

namespace Events
{
    public struct EventParam
    {
        public string EventString;
        public int EventInteger;
        public float EventFloat;
        public bool EventBoolean;
        public ApplicationState EventApplicationState;
        public PlayerSpawnPoint[] EventPlayerSpawnPoints;
        public Exhibit EventExhibit;
        public AudioClip EventAudioClip;
    }
}