using UnityEngine;
using UnityEngine.Pool;

namespace EW_Framework.Core.ObjectPool.Base
{
    /// <summary>
    /// 挂载在对象池中的对象上，用于管理对象池中的对象
    /// </summary>
    public class PoolItem : MonoBehaviour
    {
        public IObjectPool<GameObject> Pool;
    }
}
