using EW_Framework.Core.SOEventBus.Base;
using UnityEngine;

namespace EW_Framework.Core.SOEventBus.DataTypeDrivenChannel
{
    [CreateAssetMenu(fileName = "EW_BoolEventChannel", menuName = "EW_Framework/Event Channels/Bool")]
    public class BoolEventChannelSO : GameEventSO<bool> { }
}
