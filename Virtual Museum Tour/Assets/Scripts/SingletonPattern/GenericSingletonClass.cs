using UnityEngine;

namespace SingletonPattern
{
    /// <summary>
    /// The class describes the singleton pattern (used for the ApplicationController).
    /// It is based on the code of Bunny83 from the Unity forum (https://answers.unity.com/questions/1408574/destroying-and-recreating-a-singleton.html).
    /// </summary>
    /// <typeparam name="T">Class that is a component and should implement the singleton pattern.</typeparam>
    public class GenericSingletonClass<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        /// <summary>
        /// Public property that returns the singleton instance of the class.
        /// If the instance does not exist, an object containing the component is searched for within the scene.
        /// If no object is found, a new GameObject is created with the component and assigned to the local instance.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance; // if _instance isn't null, return current instance
                
                _instance = FindObjectOfType<T>(); // find _instance in scene
                if (_instance != null) return _instance; // if found, return it
                
                var obj = new GameObject // create new gameObject
                {
                    name = typeof(T).Name // set name of gameObject to component name
                };
                _instance = obj.GetComponent<T>(); // attach component to gameObject
                return _instance; // return new created instance
            }
        }

        /// <summary>
        /// Sets the inheriting class as a local instance if, none exists.
        /// If the instance is already set, the duplicate object is destroyed.
        /// </summary>
        public virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject); // set this object to not be destroyed when loading a new scene
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}