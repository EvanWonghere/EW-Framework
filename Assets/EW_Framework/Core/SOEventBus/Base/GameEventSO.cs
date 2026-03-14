using System;
using System.Collections.Generic;
using UnityEngine;

namespace EW_Framework.Core.SOEventBus.Base
{
    public abstract class GameEventSO<T> : ScriptableObject
    {
        [TextArea]
        [Tooltip("Description of the event.")]
        public string description;
        // 组合安全内核
        private readonly SafeEvent<T> _safeEvent = new();

        public void Raise(T value) => _safeEvent.Raise(value);
        public void RegisterListener(Action<T> listener) => _safeEvent.Register(listener);
        public void UnregisterListener(Action<T> listener) => _safeEvent.Unregister(listener);

#if UNITY_EDITOR
        public int ListenerCount => _safeEvent.GetListeners().Count;
        public IReadOnlyCollection<Action<T>> Listeners => _safeEvent.GetListeners();

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
