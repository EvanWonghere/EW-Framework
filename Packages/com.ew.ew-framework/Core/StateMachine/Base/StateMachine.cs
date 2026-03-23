using System;
using System.Collections.Generic;
using UnityEngine;

namespace EW_Framework.Core.StateMachine.Base
{
    /// <summary>
    /// Generic finite state machine with cached state instances.
    /// </summary>
    public class StateMachine<T>
    {
        private readonly T _context;
        private bool _started;

        private readonly Dictionary<Type, IState<T>> _stateCache = new();
        private readonly Stack<IState<T>> _stateStack = new();

        public IState<T> CurrentState { get; private set; }

        public StateMachine(T context)
        {
            if (context is null) throw new ArgumentNullException(nameof(context));
            _context = context;
        }

        /// <summary>
        /// Start the state machine with an initial state.
        /// </summary>
        public void Start<TState>() where TState : class, IState<T>, new()
        {
            if (_started)
            {
                Debug.LogWarning("[StateMachine] Start called more than once. Switching to the requested initial state.");
                ChangeState<TState>();
                return;
            }

            _started = true;
            CurrentState = GetOrAddState<TState>();
            CurrentState.Enter(_context);
        }

        /// <summary>
        /// Switch to another state (cached by type).
        /// </summary>
        public void ChangeState<TState>() where TState : class, IState<T>, new()
        {
            if (CurrentState is TState) return;

            CurrentState?.Exit(_context);

            CurrentState = GetOrAddState<TState>();

            CurrentState.Enter(_context);
        }

        // ================= Stack-based state machine extension =================

        /// <summary>
        /// Push a new state on top (e.g. gameplay -> pause menu).
        /// The previous state is suspended; if it implements <see cref="IStackState{T}"/>, <see cref="IStackState{T}.OnPause"/> is called.
        /// </summary>
        public void PushState<TState>() where TState : class, IState<T>, new()
        {
            if (CurrentState != null)
            {
                if (CurrentState is IStackState<T> stackState)
                {
                    stackState.OnPause(_context);
                }
                _stateStack.Push(CurrentState);
            }

            CurrentState = GetOrAddState<TState>();
            CurrentState.Enter(_context);
        }

        /// <summary>
        /// Pop the current state and restore the previous state.
        /// If the restored state implements <see cref="IStackState{T}"/>, <see cref="IStackState{T}.OnResume"/> is called.
        /// </summary>
        public void PopState()
        {
            if (_stateStack.Count == 0)
            {
                Debug.LogWarning("[StateMachine] State stack is empty. PopState ignored.");
                return;
            }

            CurrentState?.Exit(_context);

            CurrentState = _stateStack.Pop();
            if (CurrentState is IStackState<T> stackState)
            {
                stackState.OnResume(_context);
            }
        }

        // ======================================================

        /// <summary>
        /// Drive the current state's Update.
        /// </summary>
        public void Update()
        {
            CurrentState?.Update(_context);
        }

        private TState GetOrAddState<TState>() where TState : class, IState<T>, new()
        {
            Type type = typeof(TState);

            if (_stateCache.TryGetValue(type, out var state))
            {
                if (state is TState typedState) return typedState;
                throw new InvalidOperationException($"[StateMachine] Cached state type mismatch. Key={type.FullName}, Value={state?.GetType().FullName}");
            }

            var newState = new TState();
            _stateCache[type] = newState;

            return newState;
        }

        /// <summary>
        /// Clear current state, stack and cached state instances.
        /// </summary>
        public void Clear()
        {
            CurrentState = null;
            _stateStack.Clear();
            _stateCache.Clear();
            _started = false;
        }
    }
}