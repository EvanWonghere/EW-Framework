using UnityEngine;
using UnityEngine.InputSystem;
using EW_Framework.Core.SharedVariables.DataTypeDrivenVariable;

namespace EW_Framework.Core.SharedVariables.Examples
{
    /// <summary>
    /// 展示如何修改共享变量：通过脚本对 SharedVariableSO.Value 赋值，会触发所有监听者的 OnValueChanged。
    /// 使用新 Input System：运行时 Q/E 修改 Int，W/S 修改 Float。
    /// </summary>
    public class SharedVariableModifierExample : MonoBehaviour
    {
        [TextArea]
        [Tooltip("Description of the shared variable modifier.")]
        public string description = "This is a shared variable modifier example.\n Press Q/E to modify the shared int, and W/S to modify the shared float (Input System).";
        [SerializeField] private SharedIntSO sharedInt;
        [SerializeField] private SharedFloatSO sharedFloat;
        [SerializeField] private int intStep = 1;
        [SerializeField] private float floatStep = 0.5f;

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (sharedInt != null)
            {
                if (keyboard.qKey.wasPressedThisFrame)
                    sharedInt.Value -= intStep;
                if (keyboard.eKey.wasPressedThisFrame)
                    sharedInt.Value += intStep;
            }

            if (sharedFloat != null)
            {
                if (keyboard.wKey.wasPressedThisFrame)
                    sharedFloat.Value -= floatStep;
                if (keyboard.sKey.wasPressedThisFrame)
                    sharedFloat.Value += floatStep;
            }
        }
    }
}
