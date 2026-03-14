using EW_Framework.Core.SOEventBus.Base;
using UnityEngine;

namespace EW_Framework.Core.SOEventBus.DataTypeDrivenChannel
{
    // Listeners
    public class IntEventListener : GameEventListener<int> { }
    public class BoolEventListener : GameEventListener<bool> { }
    public class FloatEventListener : GameEventListener<float> { }
    public class StringEventListener : GameEventListener<string> { }
    public class ColorEventListener : GameEventListener<Color> { }
    public class Vector2EventListener : GameEventListener<Vector2> { }
    public class Vector3EventListener : GameEventListener<Vector3> { }
    public class QuaternionEventListener : GameEventListener<Quaternion> { }
    public class TransformEventListener : GameEventListener<Transform> { }

    // Raisers
    public class IntEventRaiser : GameEventRaiser<int> { }
    public class BoolEventRaiser : GameEventRaiser<bool> { }
    public class FloatEventRaiser : GameEventRaiser<float> { }
    public class StringEventRaiser : GameEventRaiser<string> { }
    public class ColorEventRaiser : GameEventRaiser<Color> { }
    public class Vector2EventRaiser : GameEventRaiser<Vector2> { }
    public class Vector3EventRaiser : GameEventRaiser<Vector3> { }
    public class QuaternionEventRaiser : GameEventRaiser<Quaternion> { }
    public class TransformEventRaiser : GameEventRaiser<Transform> { }
}