/// <summary>
/// Interface for objects that can be pooled
/// </summary>
namespace EW_Framework.Core.ObjectPool.Base
{
    public interface IPoolable
    {
        /// <summary>
        /// Called when the object is spawned from the pool
        /// </summary>
        void OnSpawn();

        /// <summary>
        /// Called when the object is despawned to the pool
        /// </summary>
        void OnDespawn();
    }
}
