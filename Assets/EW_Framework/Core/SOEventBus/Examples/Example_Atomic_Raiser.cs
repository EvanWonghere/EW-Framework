using UnityEngine;
using EW_Framework.Core.SOEventBus.Base;

namespace EW_Framework.Core.SOEventBus.Examples
{
    /// <summary>
    /// 原子 SO 模式示例：持有无参通道引用，对外提供 Raise()，可由 UI 按钮或按键调用。
    /// 与 Example_Atomic_Listener 搭配使用，二者 channel 指向同一 GameEventSO 资产。
    /// </summary>
    public class Example_Atomic_Raiser : MonoBehaviour
    {
        [SerializeField] private GameEventSO channel;

        /// <summary>
        /// 触发通道事件。可绑定到 Button.onClick 或在 Update 中按某键调用。
        /// </summary>
        public void Raise()
        {
            if (channel == null) return;
            channel.Raise();
        }
    }
}
