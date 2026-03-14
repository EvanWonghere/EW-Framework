#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using EW_Framework.Core.SOEventBus.Base;

namespace EW_Framework.Core.SOEventBus.Editor
{
    /// <summary>
    /// Custom editor for the GameEventSO class.
    /// </summary>
    [CustomEditor(typeof(GameEventSO))]
    public class GameEventSOEditor : UnityEditor.Editor
    {
        /// <summary>
        /// OnInspectorGUI is called when the inspector is drawn.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // Get the GameEventSO instance.
            var gameEvent = (GameEventSO)target;

            // Add a space between the inspector and the button.
            EditorGUILayout.Space();

            // Enable the button if the game is playing.
            GUI.enabled = Application.isPlaying;

            // Trigger the event.
            if (GUILayout.Button("▶ Trigger Event (Raise)", GUILayout.Height(30)))
            {
                gameEvent.Raise();
            }

            // Reset the button to enabled.
            GUI.enabled = true;

            // Show the number of active listeners.
            EditorGUILayout.LabelField($"Active Listeners: {gameEvent.ListenerCount}", EditorStyles.boldLabel);
        }
    }
}
#endif