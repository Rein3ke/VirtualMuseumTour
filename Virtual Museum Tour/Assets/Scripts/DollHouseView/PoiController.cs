using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DollHouseView
{
    public class PoiController : MonoBehaviour
    {
        private List<IPoi> _poiInheritingObjects;
        
        private void Awake()
        {
            _poiInheritingObjects = new List<IPoi>();
        }

        private void Start()
        {
            ReloadPoiInheritingScripts();

            foreach (var poiInheritingObject in _poiInheritingObjects)
            {
                poiInheritingObject.InstantiatePoi();
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += SceneManager_OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= SceneManager_OnSceneLoaded;
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
        
        private void SceneManager_OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("ExhibitionMainScene"))
            {
                
            }
        }
    }
}