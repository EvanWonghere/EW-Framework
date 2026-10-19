using UnityEngine;
using EW_Framework.Core.SOEventBus.Base;

namespace EW_Framework.Core.SOEventBus.Examples
{
    /// <summary>
    /// 原子 SO 模式示例：订阅一条无参通道，OnEnable 注册、OnDisable 注销，收到时打印日志。
    /// 与 Example_Atomic_Raiser 搭配使用，二者 channel 指向同一 GameEventSO 资产。
    /// </summary>
    public class Example_Atomic_Listener : MonoBehaviour
    {
        [SerializeField] private GameEventSO channel;

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

        private void OnRaised()
        {
            Debug.Log($"[Example_Atomic_Listener] Event received on {gameObject.name}");
        }
    }
}
