using UnityEngine;

namespace EW_Framework.Core.Singleton
{
    /// <summary>
    /// Persistent MonoBehaviour singleton
    /// Automatically calls DontDestroyOnLoad, achieving true global uniqueness and cross-scene persistence
    /// </summary>
    public abstract class PersistentMonoSingleton<T> : MonoSingleton<T> where T : PersistentMonoSingleton<T>
    {
        protected override void Awake()
        {
            base.Awake();
            
            // Ensure only the true Instance can be marked as not destroyed
            // Avoid manually placing extra instances in the scene being marked for destruction before being destroyed
            if (Instance == this)
            {
                // If the parent node is not empty, DontDestroyOnLoad will throw an error, so it is forcibly moved to the root node
                if (transform.parent != null)
                {
                    transform.SetParent(null);
                }
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}