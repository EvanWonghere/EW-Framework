using UnityEngine;
using System;
using EW_Framework.Core.ObjectPool.Base;

namespace EW_Framework.Core.TimerSystem
{
    /// <summary>
    /// Timer class
    /// </summary>
    public class Timer : IReference<Timer>
    {
        // Public properties
        public float Duration { get; private set; }
        public float TimeElapsed { get; private set; }
        public bool IsLooping { get; private set; }
        public bool UseUnscaledTime { get; private set; }
        public bool IsPaused { get; private set; }
        public bool IsCancelled { get; private set; }
        public bool IsDone { get; private set; }

        // Callback events
        private Action _onComplete;
        private Action<float> _onUpdate; // Optional: return progress (0~1) for smooth animation

        /// <summary>
        /// Initialize the timer (called by Manager)
        /// <see langword="if"/> <paramref name="duration"/> <see langword="is"/> <see langword="0"/> <see langword="and"/> <paramref name="isLooping"/> <see langword="is"/> <see langword="true"/>, it will be called each frame.
        /// </summary>
        public void Initialize(float duration, Action onComplete, bool isLooping, bool useUnscaledTime, Action<float> onUpdate = null)
        {
            Duration = duration;
            _onComplete = onComplete;
            IsLooping = isLooping;
            UseUnscaledTime = useUnscaledTime;
            _onUpdate = onUpdate;

            TimeElapsed = 0f;
            IsPaused = false;
            IsCancelled = false;
            IsDone = false;
        }

        /// <summary>
        /// Update called by Manager
        /// </summary>
        public void Tick(float deltaTime)
        {
            if (IsPaused || IsCancelled || IsDone) return;

            TimeElapsed += deltaTime;

            // Trigger progress callback (e.g. fill progress bar in UI)
            float progress = Duration <= 0f ? 1f : Mathf.Clamp01(TimeElapsed / Duration);
            _onUpdate?.Invoke(progress);

            // Time's up!
            if (TimeElapsed >= Duration)
            {
                _onComplete?.Invoke();

                if (IsLooping)
                {
                    TimeElapsed -= Duration;
                }
                else
                {
                    IsDone = true;
                }
            }
        }

        // ================= Control API for business layer =================

        public void Pause() => IsPaused = true;
        public void Resume() => IsPaused = false;

        /// <summary>
        /// Cancel the timer (Manager will recycle it in the next frame)
        /// </summary>
        public void Cancel()
        {
            IsCancelled = true;
            _onComplete = null;
            _onUpdate = null;
        }

        // ================= IReference<Timer> implementation =================

        public void OnReturnPool()
        {
            IsCancelled = false;
            IsPaused = false;
            IsDone = false;
            TimeElapsed = 0f;
            _onComplete = null;
            _onUpdate = null;
        }
    }
}
