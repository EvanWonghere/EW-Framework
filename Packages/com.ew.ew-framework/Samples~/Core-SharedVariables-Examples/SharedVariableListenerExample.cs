using UnityEngine;
using TMPro;
using EW_Framework.Core.SharedVariables.DataTypeDrivenVariable;

namespace EW_Framework.Core.SharedVariables.Examples
{
    /// <summary>
    /// 展示如何监听共享变量的变化：订阅 OnValueChanged，在值变化时打印日志。
    /// 将需要监听的 SharedIntSO/SharedFloatSO 等拖到 Inspector，运行后修改该变量即可看到控制台输出。
    /// </summary>
    public class SharedVariableListenerExample : MonoBehaviour
    {
        [SerializeField] private SharedIntSO sharedInt;
        [SerializeField] private SharedFloatSO sharedFloat;
        [SerializeField] private TextMeshProUGUI intTextMeshProUGUI;
        [SerializeField] private TextMeshProUGUI floatTextMeshProUGUI;
        [SerializeField] private string intFormat = "SharedInt: {0}";
        [SerializeField] private string floatFormat = "SharedFloat: {0}";

        private void OnEnable()
        {
            if (sharedInt != null)
                sharedInt.OnValueChanged += OnIntChanged;
            if (sharedFloat != null)
                sharedFloat.OnValueChanged += OnFloatChanged;
        }

        private void OnDisable()
        {
            if (sharedInt != null)
                sharedInt.OnValueChanged -= OnIntChanged;
            if (sharedFloat != null)
                sharedFloat.OnValueChanged -= OnFloatChanged;
        }

        private void Start()
        {
            if (sharedInt != null && intTextMeshProUGUI != null)
                intTextMeshProUGUI.text = string.Format(intFormat, sharedInt.Value);
            if (sharedFloat != null && floatTextMeshProUGUI != null)
                floatTextMeshProUGUI.text = string.Format(floatFormat, sharedFloat.Value);
        }

        private void OnIntChanged(int value)
        {
            if (intTextMeshProUGUI != null)
                intTextMeshProUGUI.text = string.Format(intFormat, value);
        }

        private void OnFloatChanged(float value)
        {
            if (floatTextMeshProUGUI != null)
                floatTextMeshProUGUI.text = string.Format(floatFormat, value);
        }
    }
}
