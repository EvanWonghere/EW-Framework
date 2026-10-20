using UnityEngine;
using EW_Framework.Core.ObjectPool.Base;

namespace EW_Framework.Core.ObjectPool.Examples
{
    /// <summary>
    /// 示例：实现 IPoolable，在从池中取出与回收时打印日志，并可选择高亮显示（便于观察复用）。
    /// 挂在需参与对象池的预制体根节点上，与 SyncPoolSpawnerExample 搭配使用。
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class PooledObjectExample : MonoBehaviour, IPoolable
    {
        [Header("Debug")]
        [Tooltip("是否在 OnSpawn/OnDespawn 时打印日志")]
        [SerializeField] private bool logLifecycle = true;
        [Tooltip("是否在 OnSpawn 时着色，便于观察实例复用：首次创建为 newInstanceColor，之后从池中复用为 reusedColor")]
        [SerializeField] private bool highlightOnSpawn = true;
        [SerializeField] private Color newInstanceColor = new Color(1f, 0.55f, 0.15f);
        [SerializeField] private Color reusedColor = new Color(0.3f, 0.85f, 1f);

        private Renderer _renderer;
        private Color _originalColor;
        private int _spawnCount;
        private static readonly int ColorProperty = Shader.PropertyToID("_BaseColor");

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            if (_renderer != null && _renderer.sharedMaterial != null)
            {
                if (_renderer.sharedMaterial.HasProperty(ColorProperty))
                    _originalColor = _renderer.sharedMaterial.GetColor(ColorProperty);
                else
                    _originalColor = _renderer.sharedMaterial.color;
            }
        }

        public void OnSpawn()
        {
            if (logLifecycle)
                Debug.Log($"[PooledObjectExample] OnSpawn: {gameObject.name} (instanceId={GetInstanceID()})");

            _spawnCount++;
            if (highlightOnSpawn && _renderer != null && _renderer.material != null)
            {
                Color color = _spawnCount == 1 ? newInstanceColor : reusedColor;
                if (_renderer.material.HasProperty(ColorProperty))
                    _renderer.material.SetColor(ColorProperty, color);
                else
                    _renderer.material.color = color;
            }
        }

        public void OnDespawn()
        {
            if (logLifecycle)
                Debug.Log($"[PooledObjectExample] OnDespawn: {gameObject.name} (instanceId={GetInstanceID()})");

            if (_renderer != null && _renderer.material != null)
            {
                if (_renderer.material.HasProperty(ColorProperty))
                    _renderer.material.SetColor(ColorProperty, _originalColor);
                else
                    _renderer.material.color = _originalColor;
            }
        }
    }
}
