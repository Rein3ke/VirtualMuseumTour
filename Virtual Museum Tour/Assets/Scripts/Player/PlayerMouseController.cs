using Interface;
using UnityEngine;

namespace Player
{
    public class PlayerMouseController : MonoBehaviour
    {
        [SerializeField] private float mouseSensitivity = 100f;
        [SerializeField] private Transform playerBody;

        private float _xRotation;
        private bool _shouldMove = true;

        private void Start()
        {
            ExhibitDetailsUserInterface.Instance.OnVisibilityChange += ExhibitDetailsUserInterface_OnVisibilityChange;
            //Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            if (!_shouldMove) return;
            
            var mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            var mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }

        private void OnDestroy()
        {
            if (ExhibitDetailsUserInterface.Instance)
            {
                ExhibitDetailsUserInterface.Instance.OnVisibilityChange -= ExhibitDetailsUserInterface_OnVisibilityChange;
            }
        }

        #region Event Handling

        private void ExhibitDetailsUserInterface_OnVisibilityChange(object sender, OnVisibilityChangeEventArgs e)
        {
            _shouldMove = !e.IsVisible;
        }

        #endregion
    }
}
