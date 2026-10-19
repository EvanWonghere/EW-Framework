using UnityEngine;

namespace EW_Framework.Core.SOEventBus.Examples
{
    /// <summary>
    /// 聚合 SO 模式示例：引用 Example_AggregatedChannelSO，OnEnable 注册、OnDisable 注销，
    /// 并暴露 Raise* 方法便于在 Inspector 或代码中触发测试。
    /// </summary>
    public class Example_Aggregated_Consumer : MonoBehaviour
    {
        [SerializeField] private Example_AggregatedChannelSO channel;

        private void OnEnable()
        {
            if (channel == null) return;
            channel.OnRequestStart.Register(OnRequestStart);
            channel.OnRequestStop.Register(OnRequestStop);
            channel.OnValueChanged.Register(OnValueChanged);
        }

        private void OnDisable()
        {
            if (channel == null) return;
            channel.OnRequestStart.Unregister(OnRequestStart);
            channel.OnRequestStop.Unregister(OnRequestStop);
            channel.OnValueChanged.Unregister(OnValueChanged);
        }

        private void OnRequestStart() => Debug.Log($"[Example_Aggregated] OnRequestStart on {gameObject.name}");
        private void OnRequestStop() => Debug.Log($"[Example_Aggregated] OnRequestStop on {gameObject.name}");
        private void OnValueChanged(int value) => Debug.Log($"[Example_Aggregated] OnValueChanged={value} on {gameObject.name}");

        /// <summary> 供测试或 UI 按钮调用。 </summary>
        public void RaiseRequestStart() => channel?.OnRequestStart.Raise();
        /// <summary> 供测试或 UI 按钮调用。 </summary>
        public void RaiseRequestStop() => channel?.OnRequestStop.Raise();
        /// <summary> 供测试或 UI 按钮调用。 </summary>
        public void RaiseValueChanged(int value) => channel?.OnValueChanged.Raise(value);
    }
}
