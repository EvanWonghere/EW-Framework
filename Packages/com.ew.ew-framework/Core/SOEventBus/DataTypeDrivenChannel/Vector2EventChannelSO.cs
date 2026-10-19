using EW_Framework.Core.SOEventBus.Base;
using UnityEngine;

namespace EW_Framework.Core.SOEventBus.DataTypeDrivenChannel
{
    [CreateAssetMenu(fileName = "EW_Vector2EventChannel", menuName = "EW_Framework/Event Channels/Vector2")]
    public class Vector2EventChannelSO : GameEventSO<Vector2> { }
}
