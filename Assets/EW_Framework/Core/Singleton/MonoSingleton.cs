using UnityEngine;

namespace EW_Framework.Core.Singleton
{
    /// <summary>
    /// Regular MonoBehaviour singleton
    /// Exists with scene loading, destroyed when scene is unloaded
    /// </summary>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance;
        private static readonly object _lock = new object();
        private static bool _isQuitting = false;

        public static T Instance
        {
            get
            {
                // Defense against "ghost object" bug when exiting Unity Editor playback
                if (_isQuitting)
                {
                    Debug.LogWarning($"[MonoSingleton] Application is exiting, no longer returning instance of {typeof(T).Name}.");
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        // 1. Try to find in the scene (note: Unity 2023+ recommends using FindFirstObjectByType)
                        _instance = Object.FindFirstObjectByType<T>();

                        // 2. If not found, automatically create one
                        if (_instance == null)
                        {
                            var singletonObject = new GameObject();
                            _instance = singletonObject.AddComponent<T>();
                            singletonObject.name = $"[Singleton] {_instance.GetType().Name}";
                        }
                    }
                    return _instance;
                }
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else if (_instance != this)
            {
                Debug.LogWarning($"[MonoSingleton] Multiple {typeof(T).Name} found in scene! Destroying extra instances.");
                Destroy(gameObject);
            }
        }

        protected virtual void OnApplicationQuit()
        {
            _isQuitting = true;
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}