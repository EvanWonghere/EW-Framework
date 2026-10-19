using EW_Framework.Core.SOEventBus.Base;
using UnityEngine;

namespace EW_Framework.Core.SOEventBus.DataTypeDrivenChannel
{
    [CreateAssetMenu(fileName = "EW_TransformEventChannel", menuName = "EW_Framework/Event Channels/Transform")]
    public class TransformEventChannelSO : GameEventSO<Transform> { }
}
