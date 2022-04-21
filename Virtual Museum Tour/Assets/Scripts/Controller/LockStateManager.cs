using UnityEngine;

namespace Controller
{
    /// <summary>
    /// Sets a desired CursorLockMode and thereby a pause state that can be checked by other classes.
    /// </summary>
    public class LockStateManager : MonoBehaviour
    {
        #region Properties

        /// <summary>
        /// Is true, if the CursorLockMode is set to CursorLockMode.Locked. Otherwise false.
        /// It is used to check if the controls (e.g. movement controls) should be disabled or not.
        /// </summary>
        public static bool IsPaused { get; private set; }

        /// <summary>
        /// Cache for the current CursorLockMode.
        /// </summary>
        private static CursorLockMode LockMode { get; set; }

        #endregion

        /// <summary>
        /// Gets a CursorLockMode and updates the Cursor.lockState.
        /// If the CursorLockMode is set to None, the "IsPaused" status is set to true.
        /// </summary>
        /// <param name="lockMode">Desired CursorLockMode (Can be None or Locked).</param>
        public static void SetInternLockState(CursorLockMode lockMode)
        {
            if (LockMode == lockMode) return; // If the desired lockMode is already set, do nothing.

            LockMode = lockMode;
            IsPaused = (LockMode == CursorLockMode.None);
            
            Cursor.lockState = LockMode;
        }
    }
}
