using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controller
{
    public class SceneController : MonoBehaviour
    {
        // static members
        public static SceneController Instance { get; private set; }

        // serialize fields
        
        // members
        
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            SceneManager.LoadScene("Scenes/TestMuseum", LoadSceneMode.Additive);
        }
    }
}
