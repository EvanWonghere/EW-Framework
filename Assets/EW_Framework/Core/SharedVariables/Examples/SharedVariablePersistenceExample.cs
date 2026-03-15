using UnityEngine;
using UnityEngine.InputSystem;
using EW_Framework.Core.SharedVariables.DataTypeDrivenVariable;

namespace EW_Framework.Core.SharedVariables.Examples
{
    /// <summary>
    /// 展示可选持久化：将需要参与存档的变量（如带 saveKey 的 SharedIntSO）拖入列表，
    /// 仅列表中的、且 SaveKey 非空的变量会被保存/加载。使用新 Input System：运行时 F5 存档，F9 读档。
    /// </summary>
    public class SharedVariablePersistenceExample : MonoBehaviour
    {
        [TextArea]
        [Tooltip("Description of the shared variable persistence example.")]
        public string description = "This is a shared variable persistence example.\n Press F5 to save, and F9 to load (Input System).";

        [SerializeField, Tooltip("仅 SaveKey 非空的项会被持久化")]
        private SharedIntSO[] saveableInts;
        [SerializeField, Tooltip("仅 SaveKey 非空的项会被持久化")]
        private SharedFloatSO[] saveableFloats;

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (keyboard.f5Key.wasPressedThisFrame)
            {
                Save();
                Debug.Log("[Persistence] 已存档 (F5)");
            }
            if (keyboard.f9Key.wasPressedThisFrame)
            {
                Load();
                Debug.Log("[Persistence] 已读档 (F9)");
            }
        }

        private void Save()
        {
            SimpleSaveLoadHelper.Save(CollectSaveables());
        }

        private void Load()
        {
            SimpleSaveLoadHelper.Load(CollectSaveables());
        }

        private System.Collections.Generic.IEnumerable<Base.ISaveable> CollectSaveables()
        {
            if (saveableInts != null)
            {
                foreach (var s in saveableInts)
                    if (s != null) yield return s;
            }
            if (saveableFloats != null)
            {
                foreach (var s in saveableFloats)
                    if (s != null) yield return s;
            }
        }
    }
}
