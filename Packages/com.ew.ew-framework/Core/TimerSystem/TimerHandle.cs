using System;

namespace EW_Framework.Core.TimerSystem
{
    /// <summary>
    /// Handle returned by <see cref="TimerManager.Register"/>.
    /// Timers are pooled, so the instance behind a handle is reused after it completes or is cancelled. The handle
    /// remembers the timer's version at registration; once the timer is recycled the handle becomes invalid, its
    /// control calls do nothing and its getters return default values. Holding on to an old handle is therefore safe.
    /// </summary>
    public readonly struct TimerHandle : IEquatable<TimerHandle>
    {
        private readonly Timer _timer;
        private readonly int _version;

        internal TimerHandle(Timer timer)
        {
            _timer = timer;
            _version = timer.Version;
        }

        /// <summary>
        /// True until the timer is recycled (by the TimerManager sweep after it completes or is cancelled, or by
        /// ClearAllTimers). A completed timer stays valid with IsDone set until that sweep.
        /// </summary>
        public bool IsValid => _timer != null && _timer.Version == _version;

        /// <summary>Valid, not completed and not cancelled.</summary>
        public bool IsActive => IsValid && !_timer.IsDone && !_timer.IsCancelled;

        public float Duration => IsValid ? _timer.Duration : 0f;
        public float TimeElapsed => IsValid ? _timer.TimeElapsed : 0f;
        public bool IsLooping => IsValid && _timer.IsLooping;
        public bool UseUnscaledTime => IsValid && _timer.UseUnscaledTime;
        public bool IsPaused => IsValid && _timer.IsPaused;
        public bool IsCancelled => IsValid && _timer.IsCancelled;
        public bool IsDone => IsValid && _timer.IsDone;

        public void Pause()
        {
            if (IsValid) _timer.Pause();
        }

        public void Resume()
        {
            if (IsValid) _timer.Resume();
        }

        public void Cancel()
        {
            if (IsValid) _timer.Cancel();
        }

        public bool Equals(TimerHandle other) => _timer == other._timer && _version == other._version;
        public override bool Equals(object obj) => obj is TimerHandle other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(_timer, _version);
        public static bool operator ==(TimerHandle a, TimerHandle b) => a.Equals(b);
        public static bool operator !=(TimerHandle a, TimerHandle b) => !a.Equals(b);
    }
}
