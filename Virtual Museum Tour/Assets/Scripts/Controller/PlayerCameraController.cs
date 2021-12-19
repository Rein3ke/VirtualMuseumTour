using UnityEngine;

namespace Controller
{
    public class PlayerCameraController : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;

        private void Start()
        {
            // When spawning player prefab, iterate through all cameras and deactivate everyone which isn't the player camera.
            /*foreach (var currentCamera in FindObjectsOfType<Camera>())
            {
                currentCamera.enabled = currentCamera.Equals(playerCamera);
            }*/
        }
    }
}
