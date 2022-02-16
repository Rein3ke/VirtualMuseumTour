using System;
using Events;
using Interface;
using UnityEngine;
using EventType = Events.EventType;

namespace Player
{
    public class PlayerMouseController : MonoBehaviour
    {
        #region SerializeFields

        [SerializeField] private float mouseSensitivity = 100f;
        [SerializeField] private Transform playerBody;

        #endregion

        #region Members

        private float _xRotation;
        private bool _isLocked;

        #endregion

        private void Start()
        {
            _isLocked = false;
        }

        private void OnEnable()
        {
            EventManager.StartListening(EventType.EventLockControls, SetLockState);
        }

        private void OnDisable()
        {
            EventManager.StopListening(EventType.EventLockControls, SetLockState);
        }
        
        private void SetLockState(EventParam eventParam)
        {
            _isLocked = eventParam.Param4;
        }

        private void Update()
        {
            if (_isLocked) return;
            
            var mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            var mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}
