using UnityEngine;
using EW_Framework.Core.SOEventBus.DataTypeDrivenChannel;

namespace EW_Framework.Core.SOEventBus.Examples
{
    /// <summary>
    /// 原子 SO 模式示例（带参）：订阅一条 Int 通道，OnEnable 注册、OnDisable 注销，收到时打印载荷。
    /// 与 Example_Atomic_IntRaiser 搭配使用，二者 channel 指向同一 IntEventChannelSO 资产。
    /// </summary>
    public class Example_Atomic_IntListener : MonoBehaviour
    {
        [SerializeField] private IntEventChannelSO channel;

        private void OnEnable()
        {
            if (channel == null) return;
            channel.RegisterListener(OnRaised);
        }

        private void OnDisable()
        {
            if (channel == null) return;
            channel.UnregisterListener(OnRaised);
        }

        private void OnRaised(int value)
        {
            Debug.Log($"[Example_Atomic_IntListener] Event received value={value} on {gameObject.name}");
        }
    }
}
