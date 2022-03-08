using Events;
using UnityEngine;
using EventType = Events.EventType;

namespace Player
{
    public class PlayerCameraController : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;

        private void OnEnable()
        {
            EventManager.StartListening(EventType.EventLockControls, OnLockStateChange);
        }

        private void OnDisable()
        {
            EventManager.StopListening(EventType.EventLockControls, OnLockStateChange);
        }

        private void OnLockStateChange(EventParam eventParam)
        {
            playerCamera.tag = !eventParam.EventBoolean ? "MainCamera" : "Untagged"; // If true, set the camera to untagged (Player is locked and camera isn't needed at the moment).
            Debug.Log($"Player camera tag is set to {playerCamera.tag}.");
        }
    }
}
