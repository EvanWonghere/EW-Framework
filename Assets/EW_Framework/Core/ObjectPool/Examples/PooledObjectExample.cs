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
        [Tooltip("是否在 OnSpawn 时短暂高亮（改变材质颜色），便于观察实例复用")]
        [SerializeField] private bool highlightOnSpawn = true;

        private Renderer _renderer;
        private Color _originalColor;
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

            if (highlightOnSpawn && _renderer != null && _renderer.material != null)
            {
                if (_renderer.material.HasProperty(ColorProperty))
                    _renderer.material.SetColor(ColorProperty, Color.Lerp(_originalColor, Color.white, 0.6f));
                else
                    _renderer.material.color = Color.Lerp(_originalColor, Color.white, 0.6f);
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
