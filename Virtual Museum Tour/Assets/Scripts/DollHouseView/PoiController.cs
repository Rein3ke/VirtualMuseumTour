using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DollHouseView
{
    public class PoiController : MonoBehaviour
    {
        private List<IPoi> _poiInheritingObjects;
        
        private void Awake()
        {
            _poiInheritingObjects = new List<IPoi>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ReloadPoiInheritingScripts();

                foreach (var poiInheritingObject in _poiInheritingObjects)
                {
                    poiInheritingObject.InstantiatePoi();
                }
            }
        }

        private void ReloadPoiInheritingScripts()
        {
            _poiInheritingObjects.Clear();
            
            _poiInheritingObjects.AddRange(FindObjectsOfType<MonoBehaviour>().OfType<IPoi>());
        }
    }
}