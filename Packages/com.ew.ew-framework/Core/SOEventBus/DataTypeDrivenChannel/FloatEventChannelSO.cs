using EW_Framework.Core.SOEventBus.Base;
using UnityEngine;

namespace EW_Framework.Core.SOEventBus.DataTypeDrivenChannel
{
    [CreateAssetMenu(fileName = "EW_FloatEventChannel", menuName = "EW_Framework/Event Channels/Float")]
    public class FloatEventChannelSO : GameEventSO<float> { }
}
