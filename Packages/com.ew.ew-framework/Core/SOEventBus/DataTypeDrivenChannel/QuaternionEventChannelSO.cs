using EW_Framework.Core.SOEventBus.Base;
using UnityEngine;

namespace EW_Framework.Core.SOEventBus.DataTypeDrivenChannel
{
    [CreateAssetMenu(fileName = "EW_QuaternionEventChannel", menuName = "EW_Framework/Event Channels/Quaternion")]
    public class QuaternionEventChannelSO : GameEventSO<Quaternion> { }
}
