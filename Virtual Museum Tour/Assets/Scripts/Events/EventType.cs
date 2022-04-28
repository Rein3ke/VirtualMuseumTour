namespace Events
{
    /// <summary>
    /// Class containing multiple strings representing events.
    /// </summary>
    public static class EventType
    {
        /// <summary>
        /// Event to trigger the loading of a scene.
        /// Two parameters: Uses a string that represents the scene to load and a bool to determine whether to load the scene additively.
        /// <example>new EventParam {EventString = "Scenes/Exhibitions/ExhibitionMainScene", EventBoolean = false};</example>
        /// </summary>
        public const string EventLoadScene = "EventLoadScene";
        /// <summary>
        /// Event to trigger an application state change.
        /// Uses an ApplicationState as parameter to set the new state.
        /// <example>new EventParam {EventApplicationState = ApplicationState.Main};</example>
        /// </summary>
        public const string EventSetState = "EventSetState";
        /// <summary>
        /// Event to notify listeners that the current state has changed.
        /// Uses an ApplicationState as parameter that represents the new state.
        /// <example>new EventParam {EventApplicationState = ApplicationState.Main};</example>
        /// </summary>
        public const string EventStateChange = "EventStateChange";
        /// <summary>
        /// Event to trigger a player teleportation.
        /// Uses either a Vector3 or a string as parameter that represents the desired PlayerSpawnPoint to be instantiated at.
        /// <example>new EventParam {EventPlayerSpawnPoint = playerSpawnPointNameString}; or new EventParam {EventPlayerSpawnPoint = playerSpawnPointObject};</example>
        /// </summary>
        public const string EventTeleportRequest = "EventTeleportRequest";
        /// <summary>
        /// Event to notify listeners that the list of PlayerSpawnPoints was updated.
        /// Uses a PlayerSpawnPoint[] as parameter that represents the list of PlayerSpawnPoints.
        /// <example>new EventParam {EventPlayerSpawnPoints = playerSpawnPoints[]};</example>
        /// </summary>
        public const string EventPlayerSpawnPointsLoaded = "EventPlayerSpawnPointsLoaded";
        /// <summary>
        /// Event to notify listeners that a player prefab was successfully initialized.
        /// Uses a bool as parameter that represents whether the player prefab was successfully initialized.
        /// <example>new EventParam {EventBoolean = true};</example>
        /// </summary>
        public const string EventSpawnPlayer = "EventSpawnPlayer";
        /// <summary>
        /// Event to notify listeners that an exhibit was selected.
        /// Uses an Exhibit object as parameter that represents the selected exhibit.
        /// <example>new EventParam {EventExhibit = exhibitObject};</example>
        /// </summary>
        public const string EventExhibitSelect = "EventExhibitSelect";
        /// <summary>
        /// Event to notify listeners that the details user interface was closed.
        /// No parameters (just send an empty struct as parameter).
        /// <example>new EventParam {};</example>
        /// </summary>
        public const string EventDetailsInterfaceClose = "EventDetailsInterfaceClose";
        /// <summary>
        /// Event to notify listeners that an desired audio clip was selected (e.g. in the details user interface).
        /// Uses an AudioClip as parameter that represents the selected audio clip.
        /// <example>new EventParam {EventAudioClip = audioClip};</example>
        /// </summary>
        public const string EventPlayAudio = "EventPlayAudio";
        /// <summary>
        /// Event to trigger the audio source to stop playing the current audio clip.
        /// No parameters (just send an empty struct as parameter).
        /// <example>new EventParam {};</example>
        /// </summary>
        public const string EventPauseAudio = "EventPauseAudio";
        /// <summary>
        /// Event to trigger the listeners to stop moving. Mostly the listeners use an internal boolean to determine whether they should continue moving or not.
        /// Uses a bool as parameter that represents whether the listeners should lock their movement or not.
        /// (True = lock movement, False = unlock movement).
        /// <example>new EventParam {EventBoolean = true};</example>
        /// </summary>
        public const string EventLockControls = "EventLockControls";
        /// <summary>
        /// Event to notify listeners that all exhibitions are initialized in the scene.
        /// No parameters (just send an empty struct as parameter).
        /// <example>new EventParam {};</example>
        /// </summary>
        public const string EventExhibitionsPlaced = "EventExhibitionsPlaced";
        /// <summary>
        /// Event to notify listeners that the doll house view was either opened or closed.
        /// Uses a bool as parameter that represents whether the doll house view was opened or closed.
        /// (True = opened, False = closed).
        /// <example>new EventParam {EventBoolean = true};</example>
        /// </summary>
        public const string EventDollHouseView = "EventDollHouseView";
        /// <summary>
        /// Event to notify listeners that an AssetBundle was loaded and unpacked.
        /// No parameters (just send an empty struct as parameter).
        /// <example>new EventParam {};</example>
        /// </summary>
        public const string EventAssetBundleLoaded = "EventAssetBundleLoaded";
    }
}