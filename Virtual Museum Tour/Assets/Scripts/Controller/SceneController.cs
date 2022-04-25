using Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using EventType = Events.EventType;

namespace Controller
{
    /// <summary>
    /// Listens for the EventLoadScene event and then performs a scene change.
    /// Scenes can be loaded additively (without unloading currently loaded scenes) or non-additively.
    /// </summary>
    public class SceneController : MonoBehaviour
    {
        #region Unity Methods

        /// <summary>
        /// Subscribes to the EventLoadScene event and calls the ExternalLoadCall method.
        /// Other classes can cause the SceneController to change the scene using this event.
        /// </summary>
        private void OnEnable()
        {
            EventManager.StartListening(EventType.EventLoadScene, ExternalLoadCall);
        }

        /// <summary>
        /// Unsubscribes from all events.
        /// </summary>
        private void OnDisable()
        {
            EventManager.StopListening(EventType.EventLoadScene, ExternalLoadCall);
        }

        #endregion

        /// <summary>
        /// Receives a path to a scene and loads it additively.
        /// Path must be relative to the project folder (e.g. "Scenes/SceneName").
        /// </summary>
        /// <param name="pathToScene">Path to the scene.</param>
        public void LoadSceneAdditive(string pathToScene)
        {
            SceneManager.LoadScene(pathToScene, LoadSceneMode.Additive);
        }

        /// <summary>
        /// Receives a path to a scene and loads it normally.
        /// Path must be relative to the project folder (e.g. "Scenes/SceneName").
        /// </summary>
        /// <param name="pathToScene">Path to the scene.</param>
        public void LoadScene(string pathToScene)
        {
            SceneManager.LoadScene(pathToScene, LoadSceneMode.Single);
        }

        #region Event Handlers

        /// <summary>
        /// Receives an eventParam struct and loads a scene defined in the struct.
        /// </summary>
        /// <param name="eventParam">Boolean (Should the scene be loaded additively or not) and string (The name of the scene to be loaded).</param>
        private void ExternalLoadCall(EventParam eventParam)
        {
            if (eventParam.EventBoolean) // if true, load the scene additively
            {
                LoadSceneAdditive(eventParam.EventString);
            }
            else // if false, load the scene normally
            {
                LoadScene(eventParam.EventString);
            }
        }

        #endregion
    }
}