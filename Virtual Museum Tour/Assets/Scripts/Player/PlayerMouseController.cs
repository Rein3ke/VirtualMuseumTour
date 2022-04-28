using Controller;
using Events;
using UnityEngine;
using EventType = Events.EventType;

namespace Player
{
    /// <summary>
    /// This class is added as a component to the player prefab and is responsible for handling the camera rotation.
    /// </summary>
    public class PlayerMouseController : MonoBehaviour
    {
        #region SerializeFields

        /// <summary>
        /// Mouse sensitivity.
        /// </summary>
        [SerializeField] private float mouseSensitivity = 100f;
        /// <summary>
        /// Reference to the player transform.
        /// </summary>
        [SerializeField] private Transform playerBody;

        #endregion

        #region Members
        
        /// <summary>
        /// Describes the rotation of the camera around the y axis.
        /// </summary>
        private float _yRotation;
        /// <summary>
        /// Boolean. False, if the player can be controlled.
        /// </summary>
        private bool _isLocked;

        #endregion

        #region Unity Methods

        /// <summary>
        /// Sets the internal lock state to "unlocked" so that the camera can be controlled with the mouse.
        /// </summary>
        private void Start()
        {
            _isLocked = false;
        }

        /// <summary>
        /// Subscribes to the LockControls event to lock or unlock the camera when triggered.
        /// </summary>
        private void OnEnable()
        {
            EventManager.StartListening(EventType.EventLockControls, SetLockState);
        }

        /// <summary>
        /// Unsubscribes from the LockControls event.
        /// </summary>
        private void OnDisable()
        {
            EventManager.StopListening(EventType.EventLockControls, SetLockState);
        }

        /// <summary>
        /// Rotates the camera around the x axis and applies the rotation to the player.
        /// The player body is rotated around the y axis. This simulates the player's head movement.
        /// </summary>
        private void Update()
        {
            if (_isLocked || LockStateManager.IsPaused) return; // if the internal lock state is set to "locked" or the game is paused, the camera is not controlled
            
            // store the calculated rotation based on the mouse sensitivity and Time.deltaTime
            var mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            var mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            _yRotation -= mouseY; // invert the mouse y axis
            _yRotation = Mathf.Clamp(_yRotation, -90f, 90f); // limit the rotation to the minimum and maximum values so that the camera can't be rotated too far up or down

            transform.localRotation = Quaternion.Euler(_yRotation, 0f, 0f); // apply the rotation to the camera
            playerBody.Rotate(Vector3.up * mouseX); // rotate the player body around the z axis based on the mouse x axis
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Sets the internal lock state when the SetLockState event is triggered.
        /// True = locked, false = unlocked.
        /// </summary>
        /// <param name="eventParam">Boolean value whether the camera can be rotated (via EventParam.EventBoolean).</param>
        private void SetLockState(EventParam eventParam)
        {
            _isLocked = eventParam.EventBoolean;
        }

        #endregion
    }
}
