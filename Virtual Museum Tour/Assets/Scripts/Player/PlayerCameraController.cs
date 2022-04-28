using Events;
using UnityEngine;
using EventType = Events.EventType;

namespace Player
{
    /// <summary>
    /// This class is added to the player prefab and sets the player camera active or inactive depending on the subscribed event.
    /// </summary>
    public class PlayerCameraController : MonoBehaviour
    {
        #region Serialized Fields

        /// <summary>
        /// Reference to the camera used in the player prefab.
        /// </summary>
        [SerializeField] private Camera playerCamera;

        #endregion

        #region Unity Methods

        /// <summary>
        /// Subscribes to the LockControls event to lock or unlock the camera when triggered.
        /// </summary>
        private void OnEnable()
        {
            EventManager.StartListening(EventType.EventLockControls, SetCameraActive);
        }
        
        /// <summary>
        /// Unsubscribes from the LockControls event.
        /// </summary>
        private void OnDisable()
        {
            EventManager.StopListening(EventType.EventLockControls, SetCameraActive);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Sets the camera active based on the given event parameter.
        /// </summary>
        /// <param name="eventParam">Boolean, if the camera should be active or not (via EventParam.EventBoolean).</param>
        private void SetCameraActive(EventParam eventParam)
        {
            playerCamera.enabled = !eventParam.EventBoolean;
        }

        #endregion
    }
}
