using System.Collections.Generic;
using System.Linq;
using Events;
using UnityEngine;
using EventType = Events.EventType;

namespace DollHouseView
{
    /// <summary>
    /// Helper controller for the doll house view. It refreshes the list of all IPoi inheritors and iterates through them to instantiate poi prefabs.
    /// </summary>
    public class PoiController : MonoBehaviour
    {
        /// <summary>
        /// List of all IPoi interface inheritors.
        /// </summary>
        private readonly List<IPoi> _poiInheritingObjects = new List<IPoi>();

        /// <summary>
        /// Start listening to the OnAssetBundleLoaded event to call the OnAssetBundleLoaded method.
        /// </summary>
        private void OnEnable()
        {
            EventManager.StartListening(EventType.EventAssetBundleLoaded, OnAssetBundleLoaded);
        }

        /// <summary>
        /// Stop listening to the OnAssetBundleLoaded event.
        /// </summary>
        private void OnDisable()
        {
            EventManager.StopListening(EventType.EventAssetBundleLoaded, OnAssetBundleLoaded);
        }

        /// <summary>
        /// Refresh the list of all IPoi interface inheritors and iterate through them to instantiate the poi prefabs.
        /// </summary>
        /// <param name="eventParam">Obsolete</param>
        private void OnAssetBundleLoaded(EventParam eventParam)
        {
            ReloadPoiInheritingScripts();

            foreach (var poiInheritingObject in _poiInheritingObjects)
            {
                poiInheritingObject.InstantiatePoi();
            }
        }

        /// <summary>
        /// Clear the list of all IPoi interface inheritors and fill it with all IPoi interface inheritors found in the scene.
        /// </summary>
        private void ReloadPoiInheritingScripts()
        {
            _poiInheritingObjects.Clear();
            
            _poiInheritingObjects.AddRange(FindObjectsOfType<MonoBehaviour>().OfType<IPoi>());
        }
    }
}