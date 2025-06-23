using System;
using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Attach to Singleton subclass to enable scene persistence
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DoNotDestroySingletonAttribute : Attribute { }

    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (HasInstance) _instance = FindAnyObjectByType<T>();
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (!HasInstance)
            {
                _instance = this as T;
                if (IsPersistent) DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this) Destroy(gameObject);
        }

        public static bool HasInstance => FindAnyObjectByType<T>() != null;

        public bool IsPersistent => typeof(T).GetCustomAttributes(typeof(DoNotDestroySingletonAttribute), true).Length > 0;

        public override string ToString()
        {
            if (!HasInstance)
                return $"Singleton<{typeof(T).Name}> (uninitialized)";

            var sceneInfo = _instance.gameObject.scene.name;
            var persistentInfo = IsPersistent ? " [DontDestroyOnLoad]" : "";

            return $"{_instance.name} Singleton<{typeof(T).Name}> in Scene '{sceneInfo}'{persistentInfo}";
        }
    }
}
