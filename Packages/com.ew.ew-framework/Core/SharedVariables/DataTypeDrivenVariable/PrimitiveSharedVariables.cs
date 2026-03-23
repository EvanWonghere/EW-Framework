using UnityEngine;
using EW_Framework.Core.SharedVariables.Base;

namespace EW_Framework.Core.SharedVariables.DataTypeDrivenVariable
{
    [CreateAssetMenu(fileName = "EW_SharedInt", menuName = "EW_Framework/Shared Variables/Int", order = 0)]
    public class SharedIntSO : SharedVariableSO<int> { }

    [CreateAssetMenu(fileName = "EW_SharedBool", menuName = "EW_Framework/Shared Variables/Bool", order = 1)]
    public class SharedBoolSO : SharedVariableSO<bool> { }

    [CreateAssetMenu(fileName = "EW_SharedFloat", menuName = "EW_Framework/Shared Variables/Float", order = 2)]
    public class SharedFloatSO : SharedVariableSO<float> { }

    [CreateAssetMenu(fileName = "EW_SharedString", menuName = "EW_Framework/Shared Variables/String", order = 3)]
    public class SharedStringSO : SharedVariableSO<string> { }

    [CreateAssetMenu(fileName = "EW_SharedColor", menuName = "EW_Framework/Shared Variables/Color", order = 4)]
    public class SharedColorSO : SharedVariableSO<Color> { }

    [CreateAssetMenu(fileName = "EW_SharedVector2", menuName = "EW_Framework/Shared Variables/Vector2", order = 5)]
    public class SharedVector2SO : SharedVariableSO<Vector2> { }

    [CreateAssetMenu(fileName = "EW_SharedVector3", menuName = "EW_Framework/Shared Variables/Vector3", order = 6)]
    public class SharedVector3SO : SharedVariableSO<Vector3> { }

    [CreateAssetMenu(fileName = "EW_SharedQuaternion", menuName = "EW_Framework/Shared Variables/Quaternion", order = 7)]
    public class SharedQuaternionSO : SharedVariableSO<Quaternion> { }
}
