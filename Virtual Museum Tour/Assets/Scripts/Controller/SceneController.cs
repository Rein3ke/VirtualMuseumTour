using System;
using System.Collections.Generic;
using Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using EventType = Events.EventType;

namespace Controller
{
    public class SceneController : MonoBehaviour
    {
        private void OnEnable()
        {
            EventManager.StartListening(EventType.EventLoadScene, ExternalLoadCall);
        }

        private void OnDisable()
        {
            EventManager.StopListening(EventType.EventLoadScene, ExternalLoadCall);
        }

        private void ExternalLoadCall(EventParam eventParam)
        {
            if (eventParam.Param4)
            {
                LoadSceneAdditive(eventParam.Param1);
            }
            else
            {
                LoadScene(eventParam.Param1);
            }
        }

        public void LoadSceneAdditive(string pathToScene)
        {
            SceneManager.LoadScene(pathToScene, LoadSceneMode.Additive);
        }

        public void LoadScene(string pathToScene)
        {
            SceneManager.LoadScene(pathToScene, LoadSceneMode.Single);
        }
    }
}