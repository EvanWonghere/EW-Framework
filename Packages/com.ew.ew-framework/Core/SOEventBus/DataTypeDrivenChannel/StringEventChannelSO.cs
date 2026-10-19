using EW_Framework.Core.SOEventBus.Base;
using UnityEngine;

namespace EW_Framework.Core.SOEventBus.DataTypeDrivenChannel
{
    [CreateAssetMenu(fileName = "EW_StringEventChannel", menuName = "EW_Framework/Event Channels/String")]
    public class StringEventChannelSO : GameEventSO<string> { }
}
