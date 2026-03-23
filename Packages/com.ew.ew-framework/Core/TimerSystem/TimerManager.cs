using UnityEngine;
using EW_Framework.Core.Singleton;
using EW_Framework.Core.ObjectPool.Manager;
using System;
using System.Collections.Generic;

namespace EW_Framework.Core.TimerSystem
{
    /// <summary>
    /// Global timer manager
    /// Zero GC, prevent concurrent modification, support cross-scene survival
    /// </summary>
    public class TimerManager : PersistentMonoSingleton<TimerManager>
    {
        // Core data structures
        /// <summary>
        /// Active timers
        /// </summary>
        private readonly List<Timer> _activeTimers = new();
        /// <summary>
        /// Timers to add
        /// Add to the active timers list in the next frame
        /// </summary>
        private readonly List<Timer> _timersToAdd = new();

        // Update is called once per frame
        private void Update()
        {
            if (_timersToAdd.Count > 0)
            {
                _activeTimers.AddRange(_timersToAdd);
                _timersToAdd.Clear();
            }

            // Reverse traversal update
            // The benefit of reverse traversal is that when we remove an element from the List in the loop, it will not affect the index of the elements that have not been traversed yet!
            for (int i = _activeTimers.Count - 1; i >= 0; i--)
            {
                if (i >= _activeTimers.Count) continue;

                Timer timer = _activeTimers[i];

                if (timer.IsCancelled || timer.IsDone) continue;

                float dt = timer.UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                timer.Tick(dt);
            }

            // 3. Unified recycling phase (Sweep)
            // Reverse traversal again, clean up the dead Timer from the list and return it to the ReferencePool
            for (int i = _activeTimers.Count - 1; i >= 0; i--)
            {
                if (_activeTimers[i].IsDone || _activeTimers[i].IsCancelled)
                {
                    ReferencePoolManager.Release(_activeTimers[i]);
                    _activeTimers.RemoveAt(i);
                }
            }
        }

        // ================= Core API =================

        /// <summary>
        /// Register a delay task
        /// </summary>
        /// <param name="duration">Delay time (seconds)</param>
        /// <param name="onComplete">Callback function when completed</param>
        /// <param name="isLooping">Whether to loop execution</param>
        /// <param name="useUnscaledTime">Whether to ignore Time.timeScale (often used when UI is paused)</param>
        /// <param name="onUpdate">Callback per frame, return progress 0~1</param>
        /// <returns>Return the handle of the timer, which can be used to cancel or pause</returns>
        public Timer Register(float duration, Action onComplete, bool isLooping = false, bool useUnscaledTime = false, Action<float> onUpdate = null)
        {
            duration = Mathf.Max(0f, duration);

            Timer newTimer = ReferencePoolManager.Acquire<Timer>();

            newTimer.Initialize(duration, onComplete, isLooping, useUnscaledTime, onUpdate);
            _timersToAdd.Add(newTimer);

            return newTimer;
        }

        // ================= Internal methods =================

#if false
        /// <summary>
        /// Recycle a timer，currently recycled by the Update method
        /// </summary>
        /// <param name="timer">The timer to recycle</param>
        /// <param name="indexToRemove">The index of the timer to remove</param>
        private void RecycleTimer(Timer timer, int indexToRemove)
        {
            _activeTimers.RemoveAt(indexToRemove);
            ReferencePoolManager.Release(timer);
        }
#endif

        /// <summary>
        /// Call when switching scenes, clear all timers (optional)
        /// </summary>
        public void ClearAllTimers()
        {
            foreach (var timer in _activeTimers)
            {
                ReferencePoolManager.Release(timer);
            }
            _activeTimers.Clear();

            foreach (var timer in _timersToAdd)
            {
                ReferencePoolManager.Release(timer);
            }
            _timersToAdd.Clear();
        }
    }
}
