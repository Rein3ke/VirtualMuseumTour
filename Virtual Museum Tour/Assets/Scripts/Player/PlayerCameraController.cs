using System;
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
            EventManager.StartListening(EventType.EventLockControls, SetCameraActive);
        }

        private void OnDisable()
        {
            EventManager.StopListening(EventType.EventLockControls, SetCameraActive);
        }

        private void SetCameraActive(EventParam eventParam)
        {
            playerCamera.enabled = !eventParam.EventBoolean;
        }
    }
}
