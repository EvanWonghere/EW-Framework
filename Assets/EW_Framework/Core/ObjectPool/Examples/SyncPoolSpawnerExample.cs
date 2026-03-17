using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using EW_Framework.Core.ObjectPool.Manager;

namespace EW_Framework.Core.ObjectPool.Examples
{
    /// <summary>
    /// 使用 SyncPoolManager 演示对象池的基本用法：
    /// - Space：在指定位置批量生成实例（来自对象池）
    /// - Backspace：回收最近生成的一个实例
    /// - C：清空当前 demo 中生成的所有实例并调用 ClearAllPools
    /// 搭配带有 PooledObjectExample（和可选 AutoDespawn）的预制体使用。
    /// </summary>
    public class SyncPoolSpawnerExample : MonoBehaviour
    {
        [Header("Prefab & Layout")]
        [Tooltip("要通过对象池生成的预制体（建议挂有 PooledObjectExample，按需挂 AutoDespawn）。")]
        [SerializeField] private GameObject prefab;

        [Tooltip("一次按 Space 生成多少个实例。")]
        [SerializeField] private int spawnCountPerBatch = 5;

        [Tooltip("生成时在 X 方向上的间距。")]
        [SerializeField] private float xSpacing = 1.5f;

        [Tooltip("生成队列的起始位置（为空则使用当前物体位置）。")]
        [SerializeField] private Transform spawnOrigin;

        [Header("Behavior")]
        [Tooltip("仅用于自动回收场景：只负责生成，不在 Backspace/C 时主动调用 Despawn，由 AutoDespawn 等组件处理回收。")]
        [SerializeField] private bool spawnOnlyAutoDespawn = false;

        [Header("Keyboard Controls")]
        [Tooltip("是否启用键盘控制（新 Input System）。")]
        [SerializeField] private bool enableKeyboard = true;

        [Tooltip("批量生成的按键。")]
        [SerializeField] private Key spawnKey = Key.Space;

        [Tooltip("回收最近一个实例的按键。")]
        [SerializeField] private Key despawnLastKey = Key.Backspace;

        [Tooltip("清空当前 demo 中的所有实例与池的按键。")]
        [SerializeField] private Key clearAllKey = Key.C;

        // 记录本 demo 通过该 Spawner 生成的实例
        private readonly List<GameObject> _spawnedInstances = new List<GameObject>();

        private void Start()
        {
            if (spawnOrigin == null)
            {
                spawnOrigin = transform;
            }
        }

        private void Update()
        {
            if (!enableKeyboard) return;

            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (keyboard[spawnKey].wasPressedThisFrame)
            {
                SpawnBatch();
            }

            if (keyboard[despawnLastKey].wasPressedThisFrame)
            {
                DespawnLast();
            }

            if (keyboard[clearAllKey].wasPressedThisFrame)
            {
                ClearAll();
            }
        }

        /// <summary>
        /// 通过 ContextMenu 或 UI Button 手动调用，批量生成实例。
        /// </summary>
        [ContextMenu("Spawn Batch")]
        public void SpawnBatch()
        {
            if (prefab == null)
            {
                Debug.LogWarning("[SyncPoolSpawnerExample] prefab is not assigned.");
                return;
            }

            if (SyncPoolManager.Instance == null)
            {
                Debug.LogWarning("[SyncPoolSpawnerExample] SyncPoolManager.Instance is null. Make sure the manager exists in the scene.");
                return;
            }

            Vector3 originPos = spawnOrigin != null ? spawnOrigin.position : transform.position;

            for (int i = 0; i < spawnCountPerBatch; i++)
            {
                Vector3 spawnPos = originPos + new Vector3(i * xSpacing, 0f, 0f);
                GameObject instance = SyncPoolManager.Instance.Spawn(prefab, spawnPos, Quaternion.identity);
                if (instance != null)
                {
                    _spawnedInstances.Add(instance);
                }
            }
        }

        /// <summary>
        /// 回收最近生成的一个实例。
        /// </summary>
        [ContextMenu("Despawn Last")]
        public void DespawnLast()
        {
            if (spawnOnlyAutoDespawn) return;
            if (SyncPoolManager.Instance == null) return;
            if (_spawnedInstances.Count == 0) return;

            int lastIndex = _spawnedInstances.Count - 1;
            GameObject instance = _spawnedInstances[lastIndex];
            _spawnedInstances.RemoveAt(lastIndex);

            if (instance != null)
            {
                SyncPoolManager.Instance.Despawn(instance);
            }
        }

        /// <summary>
        /// 清空当前 demo 中记录的实例，并调用 SyncPoolManager.ClearAllPools。
        /// </summary>
        [ContextMenu("Clear All")]
        public void ClearAll()
        {
            if (spawnOnlyAutoDespawn)
            {
                // 在仅自动回收模式下，不主动干预实例生命周期，只清理本地列表。
                _spawnedInstances.Clear();
                return;
            }

            if (SyncPoolManager.Instance == null)
            {
                _spawnedInstances.Clear();
                return;
            }

            foreach (var instance in _spawnedInstances)
            {
                if (instance != null)
                {
                    SyncPoolManager.Instance.Despawn(instance);
                }
            }

            _spawnedInstances.Clear();
            SyncPoolManager.Instance.ClearAllPools();
        }
    }
}

