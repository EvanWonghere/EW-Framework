using UnityEngine;

namespace EW_Framework.Core.SOEventBus.Base
{
    /// <summary>
    /// 可选基类：带 description 的 ScriptableObject，供自定义通道 SO（如聚合示例）继承。
    /// </summary>
    public class BaseScriptableObject : ScriptableObject
    {
        [TextArea]
        [Tooltip("Description of the event.")]
        public string description;
    }
}
