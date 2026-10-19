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

    // Channels: one ScriptableObject per file (see IntEventChannelSO.cs etc.), since Unity binds a MonoScript to a single class per file.
}