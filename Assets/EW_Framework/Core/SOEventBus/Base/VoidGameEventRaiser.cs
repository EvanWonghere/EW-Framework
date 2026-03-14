using UnityEngine;

namespace EW_Framework.Core.SOEventBus.Base
{
    public class GameEventRaiser : MonoBehaviour
    {
        [Header("Raising Event")]
        [Tooltip("The event to raise.")]
        public GameEventSO channel;

        /// <summary>
        /// Raise the event.
        /// </summary>
        public void Raise()
        {
            if (channel == null)
            {
                Debug.LogWarning($"[SOEventBus] {name}: channel is not assigned.", this);
                return;
            }
            channel.Raise();
            LogRaise();
        }
        
        /// <summary>
        /// Log the event raise when the event is raised in the editor.
        /// </summary>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void LogRaise()
        {
            Debug.Log($"Event Raised: {name}");
        }
    }
}