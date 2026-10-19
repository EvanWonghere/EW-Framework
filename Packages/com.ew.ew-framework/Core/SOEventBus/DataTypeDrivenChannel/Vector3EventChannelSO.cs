using EW_Framework.Core.SOEventBus.Base;
using UnityEngine;

namespace EW_Framework.Core.SOEventBus.DataTypeDrivenChannel
{
    [CreateAssetMenu(fileName = "EW_Vector3EventChannel", menuName = "EW_Framework/Event Channels/Vector3")]
    public class Vector3EventChannelSO : GameEventSO<Vector3> { }
}
