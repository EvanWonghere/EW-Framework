using UnityEngine.Pool;
using EW_Framework.Core.ObjectPool.Base;

namespace EW_Framework.Core.ObjectPool.Manager
{
    public static class ReferencePoolManager
    {
        /// <summary>
        /// Fast pure C# reference pool
        /// Zero GC, no dictionary lookup, absolute O(1) complexity
        /// </summary>
        private static class Cache<T> where T : class, IReference<T>, new()
        {
            public static readonly ObjectPool<T> Pool = new ObjectPool<T>(
                createFunc: () => new T(),
                actionOnGet: null,
                actionOnRelease: (obj) => obj.OnReturnPool(),
                actionOnDestroy: null,
                collectionCheck: false,
                defaultCapacity: 50,
                maxSize: 1000
            );
        }

        /// <summary>
        /// Get a pure C# object from the pool
        /// </summary>
        public static T Acquire<T>() where T : class, IReference<T>, new()
        {
            return Cache<T>.Pool.Get();
        }

        /// <summary>
        /// Return a pure C# object to the pool
        /// </summary>
        public static void Release<T>(T obj) where T : class, IReference<T>, new()
        {
            if (obj != null)
            {
                Cache<T>.Pool.Release(obj);
            }
        }

        /// <summary>
        /// Clear the memory pool of a specific type
        /// </summary>
        public static void Clear<T>() where T : class, IReference<T>, new()
        {
            Cache<T>.Pool.Clear();
        }
    }
}