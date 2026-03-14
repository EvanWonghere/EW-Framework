using UnityEngine;
using EW_Framework.Core.SOEventBus.DataTypeDrivenChannel;

namespace EW_Framework.Core.SOEventBus.Examples
{
    /// <summary>
    /// 原子 SO 模式示例（带参）：持有 Int 通道引用，提供 Raise(int)，可由 UI 或 testValue 触发。
    /// 与 Example_Atomic_IntListener 搭配使用，二者 channel 指向同一 IntEventChannelSO 资产。
    /// </summary>
    public class Example_Atomic_IntRaiser : MonoBehaviour
    {
        [SerializeField] private IntEventChannelSO channel;
        [SerializeField] [Tooltip("用于 Inspector 或按钮触发时发送的值")]
        private int testValue = 1;

        /// <summary>
        /// 使用 Inspector 中配置的 testValue 触发通道。可绑定到 Button.onClick。
        /// </summary>
        public void RaiseWithTestValue()
        {
            if (channel == null) return;
            channel.Raise(testValue);
        }

        /// <summary>
        /// 使用指定值触发通道。
        /// </summary>
        public void Raise(int value)
        {
            if (channel == null) return;
            channel.Raise(value);
        }
    }
}
