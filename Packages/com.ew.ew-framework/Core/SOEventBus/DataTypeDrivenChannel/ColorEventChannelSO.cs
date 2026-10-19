using EW_Framework.Core.SOEventBus.Base;
using UnityEngine;

namespace EW_Framework.Core.SOEventBus.DataTypeDrivenChannel
{
    [CreateAssetMenu(fileName = "EW_ColorEventChannel", menuName = "EW_Framework/Event Channels/Color")]
    public class ColorEventChannelSO : GameEventSO<Color> { }
}
