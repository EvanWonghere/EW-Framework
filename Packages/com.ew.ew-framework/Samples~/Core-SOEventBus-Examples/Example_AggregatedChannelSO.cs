using UnityEngine;
using EW_Framework.Core.SOEventBus.Base;

namespace EW_Framework.Core.SOEventBus.Examples
{
    /// <summary>
    /// 聚合 SO 模式示例：一个 SO 承载多条事件（无参 + 带参），供业务通过同一资产订阅/发布。
    /// 必须在 OnDisable 中对所有 SafeEvent 调用 Clear()，避免 Domain Reload 后悬空引用。
    /// </summary>
    [CreateAssetMenu(fileName = "EW_Example_AggregatedChannel", menuName = "EW_Framework/Examples/Aggregated Channel")]
    public class Example_AggregatedChannelSO : BaseScriptableObject
    {
        public readonly SafeVoidEvent OnRequestStart = new();
        public readonly SafeVoidEvent OnRequestStop = new();
        public readonly SafeEvent<int> OnValueChanged = new();

#if UNITY_EDITOR
        private void OnDisable()
        {
            OnRequestStart.Clear();
            OnRequestStop.Clear();
            OnValueChanged.Clear();
        }
#endif
    }
}
