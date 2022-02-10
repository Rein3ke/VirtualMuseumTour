using System;
using UnityEngine;

namespace Controller
{
    public class LockStateManager : MonoBehaviour
    {
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}
