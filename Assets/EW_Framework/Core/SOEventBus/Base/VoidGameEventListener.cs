using UnityEngine;
using UnityEngine.Events;

namespace EW_Framework.Core.SOEventBus.Base
{
    public class GameEventListener : MonoBehaviour
    {
        [Header("Listening to Event")]
        [Tooltip("The event to listen to.")]
        public GameEventSO channel;

        [Header("Responding to Event")]
        [Tooltip("The response to the event.")]
        public UnityEvent response;

        /// <summary>
        /// Register the listener when the game object is enabled.
        /// </summary>
        private void OnEnable()
        {
            if (channel == null)
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                    Debug.LogWarning($"[SOEventBus] {name}: channel is not assigned. Listener will not receive events.", this);
#endif
                return;
            }
            channel.RegisterListener(OnEventRaised);
        }

        /// <summary>
        /// Unregister the listener when the game object is disabled.
        /// </summary>
        private void OnDisable()
        {
            if (channel == null) return;
            channel.UnregisterListener(OnEventRaised);
        }

        /// <summary>
        /// Invoke the response when the event is raised.
        /// </summary>
        private void OnEventRaised()
        {
            response?.Invoke();
        }
    }
}