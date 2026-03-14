using UnityEngine;

namespace EW_Framework.Core.SOEventBus.Base
{
    public abstract class GameEventRaiser<T> : MonoBehaviour
    {
        [Header("Raising Event")]
        [Tooltip("The event to raise.")]
        public GameEventSO<T> channel;

        /// <summary>
        /// Raise the event with the given value.
        /// </summary>
        /// <param name="value">The value to raise the event with.</param>
        public void Raise(T value)
        {
            if (channel == null)
            {
                Debug.LogWarning($"[SOEventBus] {name}: channel is not assigned.", this);
                return;
            }
            channel.Raise(value);
            LogRaise(value);
        }

        /// <summary>
        /// Log the event raise when the event is raised in the editor.
        /// </summary>
        /// <param name="value">The value to log the event raise with.</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void LogRaise(T value)
        {
            Debug.Log($"Event Raised: {name} Value: {value}");
        }
    }
}