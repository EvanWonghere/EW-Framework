using EW_Framework.Core.SOEventBus.Base;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace EW_Framework.Core.SOEventBus.DataTypeDrivenChannel
{
    // Unity Events
    [Serializable] public class UnityIntEvent : UnityEvent<int> { }
    [Serializable] public class UnityBoolEvent : UnityEvent<bool> { }
    [Serializable] public class UnityFloatEvent : UnityEvent<float> { }
    [Serializable] public class UnityStringEvent : UnityEvent<string> { }
    [Serializable] public class UnityColorEvent : UnityEvent<Color> { }
    [Serializable] public class UnityVector2Event : UnityEvent<Vector2> { }
    [Serializable] public class UnityVector3Event : UnityEvent<Vector3> { }
    [Serializable] public class UnityQuaternionEvent : UnityEvent<Quaternion> { }
    [Serializable] public class UnityTransformEvent : UnityEvent<Transform> { }

    // Channels
    [CreateAssetMenu(fileName = "EW_IntEventChannel", menuName = "EW_Framework/Event Channels/Int")]
    public class IntEventChannelSO : GameEventSO<int> { }

    [CreateAssetMenu(fileName = "EW_BoolEventChannel", menuName = "EW_Framework/Event Channels/Bool")]
    public class BoolEventChannelSO : GameEventSO<bool> { }

    [CreateAssetMenu(fileName = "EW_FloatEventChannel", menuName = "EW_Framework/Event Channels/Float")]
    public class FloatEventChannelSO : GameEventSO<float> { }

    [CreateAssetMenu(fileName = "EW_StringEventChannel", menuName = "EW_Framework/Event Channels/String")]
    public class StringEventChannelSO : GameEventSO<string> { }

    [CreateAssetMenu(fileName = "EW_ColorEventChannel", menuName = "EW_Framework/Event Channels/Color")]
    public class ColorEventChannelSO : GameEventSO<Color> { }

    [CreateAssetMenu(fileName = "EW_Vector2EventChannel", menuName = "EW_Framework/Event Channels/Vector2")]
    public class Vector2EventChannelSO : GameEventSO<Vector2> { }

    [CreateAssetMenu(fileName = "EW_Vector3EventChannel", menuName = "EW_Framework/Event Channels/Vector3")]
    public class Vector3EventChannelSO : GameEventSO<Vector3> { }

    [CreateAssetMenu(fileName = "EW_QuaternionEventChannel", menuName = "EW_Framework/Event Channels/Quaternion")]
    public class QuaternionEventChannelSO : GameEventSO<Quaternion> { }

    [CreateAssetMenu(fileName = "EW_TransformEventChannel", menuName = "EW_Framework/Event Channels/Transform")]
    public class TransformEventChannelSO : GameEventSO<Transform> { }
}