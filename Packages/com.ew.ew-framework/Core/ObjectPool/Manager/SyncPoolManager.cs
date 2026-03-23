using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using EW_Framework.Core.Singleton;
using EW_Framework.Core.ObjectPool.Base;

namespace EW_Framework.Core.ObjectPool.Manager
{
    public class SyncPoolManager : PersistentMonoSingleton<SyncPoolManager>
    {
        [Header("Pool Settings")]
        [Tooltip("The default capacity of the pool")]
        public int defaultCapacity = 10;
        [Tooltip("The maximum capacity of the pool (objects exceeding this number will be destroyed)")]
        public int maxSize = 100;

        // Core dictionary 1: Prefab -> corresponding object pool
        private readonly Dictionary<GameObject, IObjectPool<GameObject>> _prefabPools = new();

        [Header("Lifecycle Settings")]
        [Tooltip("Whether to automatically clear the object pool when the scene is unloaded? (if multiple scenes are loaded Additively, please be cautious to turn on)")]
        public bool autoClearOnSceneUnload = false;

        // Register the scene unload event of the engine
        private void OnEnable()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        // Be sure to unregister the event in OnDisable to prevent the event from being triggered after the component is destroyed, causing null pointer exceptions
        private void OnDisable()
        {
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        // Silent triggered callback function
        private void OnSceneUnloaded(Scene scene)
        {
            if (autoClearOnSceneUnload)
            {
                Debug.Log($"[SyncPoolManager] Detected scene '{scene.name}' unload, starting silent memory pool cleanup...");
                ClearAllPools();
            }
        }

        protected override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        /// Ultimate method for replacing Instantiate
        /// </summary>
        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (prefab == null) return null;

            // 1. If the dictionary does not have a pool for this Prefab, create one on the spot
            if (!_prefabPools.TryGetValue(prefab, out var pool))
            {
                pool = CreatePool(prefab);
                _prefabPools[prefab] = pool;
            }

            // 2. Get the object from the pool (the official底层 will automatically handle whether to take the old or new one)
            GameObject instance = pool.Get();

            // 3. Set Transform information
            instance.transform.SetPositionAndRotation(position, rotation);
            if (parent != null) instance.transform.SetParent(parent);

            if (!instance.TryGetComponent<PoolItem>(out var poolItem))
            {
                poolItem = instance.AddComponent<PoolItem>();
            }
            poolItem.Pool = pool;

            // 4. Trigger the custom lifecycle interface (notify the object "you are on duty")
            var poolables = instance.GetComponentsInChildren<IPoolable>();
            foreach (var p in poolables)
            {
                p.OnSpawn();
            }

            return instance;
        }

        /// <summary>
        /// Ultimate method for replacing Destroy
        /// </summary>
        public void Despawn(GameObject instance)
        {
            if (instance == null) return;

            // 1. Trigger the custom lifecycle interface (notify the object "you are off duty, reset the state")
            var poolables = instance.GetComponentsInChildren<IPoolable>();
            foreach (var p in poolables)
            {
                p.OnDespawn();
            }

            if (instance.TryGetComponent<PoolItem>(out var poolItem) && poolItem.Pool != null)
            {
                poolItem.Pool.Release(instance);
                return;
            }
            else
            {
                Destroy(instance);
            }
        }

        // Wrap the official Unity pooling logic
        private IObjectPool<GameObject> CreatePool(GameObject prefab)
        {
            // Create a parent node, keep the Hierarchy window clean
            GameObject poolParent = new GameObject($"[Pool] {prefab.name}");
            poolParent.transform.SetParent(transform);

            return new ObjectPool<GameObject>(
                createFunc: () => Instantiate(prefab, poolParent.transform), // When creating
                actionOnGet: (obj) => obj.SetActive(true),                   // When getting
                actionOnRelease: (obj) => obj.SetActive(false),              // When recycling
                actionOnDestroy: (obj) => Destroy(obj),                      // When destroying (exceeding maxSize)
                collectionCheck: false,                                      // Disable collection check (improve performance)
                defaultCapacity: defaultCapacity,
                maxSize: maxSize
            );
        }

        /// <summary>
        /// Call when switching scenes or when memory is tight, completely destroy all pools and clear the dictionary
        /// </summary>
        public void ClearAllPools()
        {
            // 1. Dispose pools to destroy all pooled GameObjects (via actionOnDestroy)
            foreach (var pool in _prefabPools.Values)
            {
                if (pool is System.IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            // 2. Clear the dictionary references
            _prefabPools.Clear();

            // 3. Clean up the empty nodes as parent nodes
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            Debug.Log("[SyncPoolManager] Synchronous object pool has been completely cleared.");
        }
    }
}