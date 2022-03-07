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
            if (eventParam.EventBoolean)
            {
                LoadSceneAdditive(eventParam.EventString);
            }
            else
            {
                LoadScene(eventParam.EventString);
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