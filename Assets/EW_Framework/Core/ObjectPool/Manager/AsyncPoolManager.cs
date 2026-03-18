using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using EW_Framework.Core.Singleton;
using EW_Framework.Core.ObjectPool.Base;

namespace EW_Framework.Core.ObjectPool.Manager
{
    // Data structure for encapsulating the pool and Addressables memory handle
    public class AsyncPoolData
    {
        public IObjectPool<GameObject> Pool;
        public AsyncOperationHandle<GameObject> PrefabHandle;
        public GameObject PoolParent;
    }

    public class AsyncPoolManager : PersistentMonoSingleton<AsyncPoolManager>
    {
        public int defaultCapacity = 10;
        public int maxSize = 100;

        // GUID -> composite structure containing the pool and memory handle
        private readonly Dictionary<string, AsyncPoolData> _asyncPools = new();
        private readonly Dictionary<string, UniTask<AsyncPoolData>> _creatingTasks = new();

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
                Debug.Log($"[AsyncPoolManager] Detected scene '{scene.name}' unload, starting silent memory pool cleanup...");
                ClearAllPools();
            }
        }

        protected override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        /// Core API for asynchronous object pool generation
        /// </summary>
        public async UniTask<GameObject> SpawnAsync(AssetReference assetRef, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (assetRef == null || !assetRef.RuntimeKeyIsValid()) return null;

            string guid = assetRef.AssetGUID;

            // 1. If the pool does not exist, asynchronously initialize it（with concurrency protection）
            if (!_asyncPools.TryGetValue(guid, out var poolData))
            {
                if (!_creatingTasks.TryGetValue(guid, out var creatingTask))
                {
                    creatingTask = CreateAsyncPool(assetRef, guid);
                    _creatingTasks[guid] = creatingTask;
                }

                try
                {
                    poolData = await creatingTask;
                }
                finally
                {
                    _creatingTasks.Remove(guid);
                }

                // If creation failed (e.g. Addressables load error), abort this spawn safely
                if (poolData == null)
                {
                    return null;
                }

                _asyncPools[guid] = poolData;
            }

            // 2. Get the object from the synchronous pool
            GameObject instance = poolData.Pool.Get();
            instance.transform.SetPositionAndRotation(position, rotation);
            if (parent != null) instance.transform.SetParent(parent);

            if (!instance.TryGetComponent<PoolItem>(out var poolItem))
            {
                poolItem = instance.AddComponent<PoolItem>();
            }
            poolItem.Pool = poolData.Pool;

            // 4. Trigger the interface
            foreach (var p in instance.GetComponentsInChildren<IPoolable>())
            {
                p.OnSpawn();
            }

            return instance;
        }

        /// <summary>
        /// API for asynchronous pool recycling (the operation itself is synchronous)
        /// </summary>
        public void Despawn(GameObject instance)
        {
            if (instance == null) return;

            foreach (var p in instance.GetComponentsInChildren<IPoolable>())
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

        private async UniTask<AsyncPoolData> CreateAsyncPool(AssetReference assetRef, string guid)
        {
            try
            {
                // Core: First asynchronously load the original prefab into memory
                var handle = Addressables.LoadAssetAsync<GameObject>(assetRef);
                GameObject loadedPrefab = await handle.WithCancellation(this.GetCancellationTokenOnDestroy());

                if (loadedPrefab == null || this == null)
                {
                    Debug.LogError($"[AsyncPoolManager] Failed to load prefab for AssetReference '{assetRef.RuntimeKey}'. Loaded prefab is null.");

                    if (handle.IsValid())
                    {
                        Addressables.Release(handle);
                    }

                    return null;
                }

                GameObject poolParent = new GameObject($"[AsyncPool] {loadedPrefab.name}");
                poolParent.transform.SetParent(transform);

                var pool = new ObjectPool<GameObject>(
                    createFunc: () => Instantiate(loadedPrefab, poolParent.transform),
                    actionOnGet: (obj) => obj.SetActive(true),
                    actionOnRelease: (obj) => obj.SetActive(false),
                    actionOnDestroy: (obj) => Destroy(obj),
                    collectionCheck: false,
                    defaultCapacity: defaultCapacity,
                    maxSize: maxSize
                );

                var poolData = new AsyncPoolData
                {
                    Pool = pool,
                    PrefabHandle = handle,
                    PoolParent = poolParent
                };

                // [Optimization]: The creator is responsible for inserting the final dictionary
                _asyncPools[guid] = poolData;
                return poolData;
            }
            catch (OperationCanceledException)
            {
                // Capture the cancellation exception (when the scene is switched and destroyed, it will enter here), silently process
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AsyncPoolManager] Exception: {ex}");
                return null;
            }
            finally
            {
                // Whether successful or not, after the creation task is completed, clean up yourself
                _creatingTasks.Remove(guid);
            }
        }

        // ================= Cleaning and memory release logic =================

        /// <summary>
        /// Completely release Addressables memory and object pool
        /// </summary>
        public void ClearAllPools()
        {
            foreach (var poolData in _asyncPools.Values)
            {
                // 1. Empty the pool of instantiated GameObject
                if (poolData.Pool is System.IDisposable disposable)
                {
                    disposable.Dispose();
                }
                
                // 2. Core: Release the resource handle of Addressables, completely unload the textures and models from memory!
                if (poolData.PrefabHandle.IsValid())
                {
                    Addressables.Release(poolData.PrefabHandle);
                }

                // 3. Destroy the empty node
                if (poolData.PoolParent != null)
                {
                    Destroy(poolData.PoolParent);
                }
            }

            _asyncPools.Clear();
            _creatingTasks.Clear();

            Debug.Log("[AsyncPoolManager] Asynchronous object pool has been cleared, Addressables memory has been released.");
        }
    }
}