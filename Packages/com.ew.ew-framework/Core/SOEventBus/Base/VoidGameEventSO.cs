using System;
using System.Collections.Generic;
using UnityEngine;

namespace EW_Framework.Core.SOEventBus.Base
{
    // Basic parameterless event bus without generics. Void Game Event SO.
    [CreateAssetMenu(
        fileName = "EW_VoidEventChannel",
        menuName = "EW_Framework/Event Channels/Void"
    )]
    public class GameEventSO : ScriptableObject
    {
        [TextArea]
        [Tooltip("Description of the event.")]
        public string description;

        private readonly SafeVoidEvent _safeEvent = new();

        public void Raise() => _safeEvent.Raise();
        public void RegisterListener(Action listener) => _safeEvent.Register(listener);
        public void UnregisterListener(Action listener) => _safeEvent.Unregister(listener);

#if UNITY_EDITOR
        /// <summary>
        /// The number of listeners for the event.
        /// </summary>
        public int ListenerCount => _safeEvent.GetListeners().Count;
        public IReadOnlyCollection<Action> Listeners => _safeEvent.GetListeners();

        /// <summary>
        /// Clear all listeners when the event is disabled.
        /// </summary>
        private void OnDisable()
        {
            _safeEvent.Clear();
        }
#endif
    }
}