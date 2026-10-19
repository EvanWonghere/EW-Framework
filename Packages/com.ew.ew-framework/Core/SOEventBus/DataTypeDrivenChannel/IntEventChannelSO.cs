using EW_Framework.Core.SOEventBus.Base;
using UnityEngine;

namespace EW_Framework.Core.SOEventBus.DataTypeDrivenChannel
{
    [CreateAssetMenu(fileName = "EW_IntEventChannel", menuName = "EW_Framework/Event Channels/Int")]
    public class IntEventChannelSO : GameEventSO<int> { }
}
