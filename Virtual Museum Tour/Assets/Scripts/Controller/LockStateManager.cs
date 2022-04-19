using UnityEngine;

namespace Controller
{
    public class LockStateManager : MonoBehaviour
    {
        public static bool IsPaused { get; private set; }

        public static CursorLockMode LockMode { get; private set; }

        private void Update()
        {
            /*if (Input.GetMouseButtonDown(0)) // left mouse click
            {
                SetInternLockState(CursorLockMode.Confined);
            }

            if (Input.GetMouseButtonDown(1)) // right mouse click
            {
                if (LockMode != CursorLockMode.None && !IsPaused)
                {
                    SetInternLockState(CursorLockMode.None);
                }
            }*/
        }

        private void OnGUI()
        {
            // Cursor.lockState = LockMode;
        }

        public static void SetInternLockState(CursorLockMode lockMode)
        {
            if (LockMode == lockMode) return;

            LockMode = lockMode;
            IsPaused = LockMode == CursorLockMode.None;
            
            Cursor.lockState = LockMode;
        }
    }
}
