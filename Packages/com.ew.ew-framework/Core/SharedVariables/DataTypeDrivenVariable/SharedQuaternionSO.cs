using UnityEngine;
using EW_Framework.Core.SharedVariables.Base;

namespace EW_Framework.Core.SharedVariables.DataTypeDrivenVariable
{
    [CreateAssetMenu(fileName = "EW_SharedQuaternion", menuName = "EW_Framework/Shared Variables/Quaternion", order = 7)]
    public class SharedQuaternionSO : SharedVariableSO<Quaternion> { }
}
