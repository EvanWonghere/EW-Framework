using UnityEngine;
using EW_Framework.Core.SOEventBus.Base;

namespace EW_Framework.Modules.AudioSystem.Runtime
{
    [CreateAssetMenu(fileName = "EW_AudioRequestChannel", menuName = "EW_Framework/Audio/AudioRequestChannel")]
    public class AudioRequestChannelSO : GameEventSO<AudioCommand> { }
}
