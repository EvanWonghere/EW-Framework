using UnityEngine;
using System.Collections;
using EW_Framework.Core.ObjectPool.Manager;

namespace EW_Framework.Core.ObjectPool.Utilities
{
    /// <summary>
    /// Attached to objects that need to be automatically recycled (e.g. particle effects, floating text, sound effects)
    /// </summary>
    public class AutoDespawn : MonoBehaviour
    {
        [Tooltip("Lifetime (seconds), automatically return to pool when time is up")]
        public float delay = 2f;

        private void OnEnable()
        {
            // Every time an object is taken out of the pool and activated, the countdown starts automatically
            StartCoroutine(DespawnRoutine());
        }

        private IEnumerator DespawnRoutine()
        {
            yield return new WaitForSeconds(delay);
            
            // Check if PoolManager is alive (to prevent errors when the game exits)
            if (SyncPoolManager.Instance != null)
            {
                SyncPoolManager.Instance.Despawn(gameObject);
            }
        }
    }
}