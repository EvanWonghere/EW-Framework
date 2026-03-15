using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace EW_Framework.Core.SharedVariables.Base {
    [Serializable]
    public class SaveDataWrapper<T> {
        public T value;
    }

    public abstract class SharedVariableSO<T> : ScriptableObject, ISaveable
    {
        [TextArea]
        [Tooltip("Description of the shared variable.")]
        public string description;

        [Header("Save Settings")]
        [Tooltip("The key of the save data, if the data does not need to be saved, leave it empty.")]
        public string saveKey;

        [Header("Data")]
        public T initialValue;
        [SerializeField] private T runtimeValue;
        public event Action<T> OnValueChanged;

        // Getter and setter for the runtime value
        public T Value
        {
            get => runtimeValue;
            set
            {
                if (!EqualityComparer<T>.Default.Equals(runtimeValue, value))
                {
                    runtimeValue = value;
                    OnValueChanged?.Invoke(runtimeValue);
                }
            }
        }

        // OnEnable to set the initial value
        private void OnEnable() => runtimeValue = initialValue;

        // ISaveable implementation
        public string SaveKey => saveKey;
        public string GetSaveData() => JsonConvert.SerializeObject(new SaveDataWrapper<T> { value = runtimeValue });
        public void LoadData(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData)) return;
            try
            {
                var wrapper = JsonConvert.DeserializeObject<SaveDataWrapper<T>>(jsonData);
                if (wrapper != null)
                    Value = wrapper.value;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SharedVariableSO] LoadData failed for SaveKey={SaveKey}: {ex.Message}", this);
            }
        }
    }
}
