using UnityEngine;
using EW_Framework.Core.SharedVariables.Base;

namespace EW_Framework.Core.SharedVariables.DataTypeDrivenVariable
{
    [CreateAssetMenu(fileName = "EW_SharedColor", menuName = "EW_Framework/Shared Variables/Color", order = 4)]
    public class SharedColorSO : SharedVariableSO<Color> { }
}
