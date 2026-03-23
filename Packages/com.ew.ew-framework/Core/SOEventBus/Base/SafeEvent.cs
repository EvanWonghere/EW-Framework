using System;
using System.Collections.Generic;

namespace EW_Framework.Core.SOEventBus.Base
{
    // Safe Event with generic payload.
    public class SafeEvent<T>
    {
        private readonly HashSet<Action<T>> _listeners = new();

        public IReadOnlyCollection<Action<T>> GetListeners() => _listeners;
        public void Raise(T value)
        {
            var snapshot = new Action<T>[_listeners.Count];
            _listeners.CopyTo(snapshot);
            foreach (var listener in snapshot)
            {
                listener?.Invoke(value);
            }
        }

        public void Register(Action<T> listener) => _listeners.Add(listener);
        public void Unregister(Action<T> listener) => _listeners.Remove(listener);
        public void Clear() => _listeners.Clear(); // Called by SO on OnDisable.
    }

    // Safe Event without payload.
    public class SafeVoidEvent
    {
        private readonly HashSet<Action> _listeners = new();
        public IReadOnlyCollection<Action> GetListeners() => _listeners;
        public void Raise()
        {
            var snapshot = new Action[_listeners.Count];
            _listeners.CopyTo(snapshot);
            foreach (var listener in snapshot)
            {
                listener?.Invoke();
            }
        }

        public void Register(Action listener) => _listeners.Add(listener);
        public void Unregister(Action listener) => _listeners.Remove(listener);
        public void Clear() => _listeners.Clear();
    }
}