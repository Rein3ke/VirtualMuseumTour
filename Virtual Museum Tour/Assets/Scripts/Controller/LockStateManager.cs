using UnityEngine;

namespace Controller
{
    public class LockStateManager : MonoBehaviour
    {
        public static bool HasFocus { get; private set; }

        private void OnApplicationFocus(bool hasFocus)
        {
            HasFocus = hasFocus;
            Cursor.lockState = HasFocus ? CursorLockMode.Confined : CursorLockMode.None;
        }
    }
}
