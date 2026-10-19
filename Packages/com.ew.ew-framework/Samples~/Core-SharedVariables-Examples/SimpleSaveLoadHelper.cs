using System.Collections.Generic;
using UnityEngine;
using EW_Framework.Core.SharedVariables.Base;

namespace EW_Framework.Core.SharedVariables.Examples
{
    /// <summary>
    /// 可选持久化：仅对 SaveKey 非空的 ISaveable 进行存储，使用 PlayerPrefs 作为后端（示例用）。
    /// 生产环境可替换为文件、云存档等。
    /// </summary>
    public static class SimpleSaveLoadHelper
    {
        private const string Prefix = "SharedVar_";

        public static void Save(IEnumerable<ISaveable> saveables)
        {
            if (saveables == null) return;
            foreach (var s in saveables)
            {
                if (s == null || string.IsNullOrEmpty(s.SaveKey)) continue;
                try
                {
                    PlayerPrefs.SetString(Prefix + s.SaveKey, s.GetSaveData());
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[SimpleSaveLoad] Save failed for key '{s.SaveKey}': {ex.Message}");
                }
            }
            PlayerPrefs.Save();
        }

        public static void Load(IEnumerable<ISaveable> saveables)
        {
            if (saveables == null) return;
            foreach (var s in saveables)
            {
                if (s == null || string.IsNullOrEmpty(s.SaveKey)) continue;
                var key = Prefix + s.SaveKey;
                if (!PlayerPrefs.HasKey(key)) continue;
                try
                {
                    s.LoadData(PlayerPrefs.GetString(key));
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[SimpleSaveLoad] Load failed for key '{s.SaveKey}': {ex.Message}");
                }
            }
        }
    }
}
