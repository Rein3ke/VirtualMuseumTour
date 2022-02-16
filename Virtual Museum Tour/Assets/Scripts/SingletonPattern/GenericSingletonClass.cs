using UnityEngine;

namespace SingletonPattern
{
    public class GenericSingletonClass<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

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

        public virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}