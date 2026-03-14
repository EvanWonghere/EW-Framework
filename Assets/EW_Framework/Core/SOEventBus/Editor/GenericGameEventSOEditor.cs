#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using EW_Framework.Core.SOEventBus.Base;
using EW_Framework.Core.SOEventBus.DataTypeDrivenChannel;

namespace EW_Framework.Core.SOEventBus.Editor
{
    /// <summary>
    /// Base editor for generic GameEventSO&lt;T&gt; channels. Provides "Trigger Event (Raise with default)"
    /// and Active Listeners count in Inspector. Each concrete channel type uses a derived class with
    /// [CustomEditor(typeof(XxxEventChannelSO))] to register this behaviour.
    /// </summary>
    public abstract class GenericGameEventSOEditorBase : UnityEditor.Editor
    {
        private Type _payloadType;
        private PropertyInfo _listenerCountProp;
        private MethodInfo _raiseMethod;

        private void CacheReflection()
        {
            if (_payloadType != null) return;
            var baseType = target.GetType().BaseType;
            if (baseType == null || !baseType.IsGenericType) return;
            var genericDef = baseType.GetGenericTypeDefinition();
            if (genericDef != typeof(GameEventSO<>)) return;
            _payloadType = baseType.GetGenericArguments()[0];
            _listenerCountProp = baseType.GetProperty("ListenerCount", BindingFlags.Public | BindingFlags.Instance);
            _raiseMethod = baseType.GetMethod("Raise", new[] { _payloadType });
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            CacheReflection();
            if (_payloadType == null || _listenerCountProp == null || _raiseMethod == null) return;

            EditorGUILayout.Space();
            GUI.enabled = Application.isPlaying;
            if (GUILayout.Button("▶ Trigger Event (Raise with default)", GUILayout.Height(30)))
            {
                object defaultVal = _payloadType.IsValueType ? Activator.CreateInstance(_payloadType) : null;
                try
                {
                    _raiseMethod.Invoke(target, new[] { defaultVal });
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            GUI.enabled = true;

            try
            {
                var count = _listenerCountProp.GetValue(target);
                EditorGUILayout.LabelField($"Active Listeners: {count}", EditorStyles.boldLabel);
            }
            catch
            {
                EditorGUILayout.LabelField("Active Listeners: —", EditorStyles.boldLabel);
            }
        }
    }

    [CustomEditor(typeof(IntEventChannelSO))]
    public class IntEventChannelSOEditor : GenericGameEventSOEditorBase { }

    [CustomEditor(typeof(BoolEventChannelSO))]
    public class BoolEventChannelSOEditor : GenericGameEventSOEditorBase { }

    [CustomEditor(typeof(FloatEventChannelSO))]
    public class FloatEventChannelSOEditor : GenericGameEventSOEditorBase { }

    [CustomEditor(typeof(StringEventChannelSO))]
    public class StringEventChannelSOEditor : GenericGameEventSOEditorBase { }

    [CustomEditor(typeof(ColorEventChannelSO))]
    public class ColorEventChannelSOEditor : GenericGameEventSOEditorBase { }

    [CustomEditor(typeof(Vector2EventChannelSO))]
    public class Vector2EventChannelSOEditor : GenericGameEventSOEditorBase { }

    [CustomEditor(typeof(Vector3EventChannelSO))]
    public class Vector3EventChannelSOEditor : GenericGameEventSOEditorBase { }

    [CustomEditor(typeof(QuaternionEventChannelSO))]
    public class QuaternionEventChannelSOEditor : GenericGameEventSOEditorBase { }

    [CustomEditor(typeof(TransformEventChannelSO))]
    public class TransformEventChannelSOEditor : GenericGameEventSOEditorBase { }
}
#endif
