using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EW_Framework.Core.TimerSystem.Examples
{
    public sealed class TimerSystemExampleDriver : MonoBehaviour
    {
        [Header("Timings")]
        [SerializeField, Min(0f)] private float _scaledDelaySeconds = 2f;
        [SerializeField, Min(0f)] private float _unscaledDelaySeconds = 2f;
        [SerializeField, Min(0f)] private float _loopIntervalSeconds = 1f;

        [Header("HUD / Logging")]
        [SerializeField, Min(1)] private int _maxLogLines = 10;

        private readonly List<Timer> _timers = new();
        private readonly List<string> _logLines = new();

        private Timer _lastTimer;
        private int _loopTickCount;

        public IReadOnlyList<Timer> Timers => _timers;
        public IReadOnlyList<string> LogLines => _logLines;
        public Timer LastTimer => _lastTimer;

        private InputAction _registerScaledDelayAction;
        private InputAction _registerUnscaledDelayAction;
        private InputAction _registerLoopAction;
        private InputAction _togglePauseLastAction;
        private InputAction _cancelLastAction;
        private InputAction _clearAllAction;
        private InputAction _toggleTimeScaleAction;

        private void Awake()
        {
            _registerScaledDelayAction = NewAction("RegisterScaledDelay", "<Keyboard>/1");
            _registerUnscaledDelayAction = NewAction("RegisterUnscaledDelay", "<Keyboard>/2");
            _registerLoopAction = NewAction("RegisterLoop", "<Keyboard>/3");
            _togglePauseLastAction = NewAction("TogglePauseLast", "<Keyboard>/p");
            _cancelLastAction = NewAction("CancelLast", "<Keyboard>/c");
            _clearAllAction = NewAction("ClearAll", "<Keyboard>/r");
            _toggleTimeScaleAction = NewAction("ToggleTimeScale", "<Keyboard>/t");

            _registerScaledDelayAction.performed += OnRegisterScaledDelayPerformed;
            _registerUnscaledDelayAction.performed += OnRegisterUnscaledDelayPerformed;
            _registerLoopAction.performed += OnRegisterLoopPerformed;
            _togglePauseLastAction.performed += OnTogglePauseLastPerformed;
            _cancelLastAction.performed += OnCancelLastPerformed;
            _clearAllAction.performed += OnClearAllPerformed;
            _toggleTimeScaleAction.performed += OnToggleTimeScalePerformed;
        }

        private void OnEnable()
        {
            Log("TimerSystem demo ready. Press 1/2/3/P/C/R/T.");

            _registerScaledDelayAction.Enable();
            _registerUnscaledDelayAction.Enable();
            _registerLoopAction.Enable();
            _togglePauseLastAction.Enable();
            _cancelLastAction.Enable();
            _clearAllAction.Enable();
            _toggleTimeScaleAction.Enable();
        }

        private void Update()
        {
            SweepDeadHandles();
        }

        private void OnDisable()
        {
            _registerScaledDelayAction.Disable();
            _registerUnscaledDelayAction.Disable();
            _registerLoopAction.Disable();
            _togglePauseLastAction.Disable();
            _cancelLastAction.Disable();
            _clearAllAction.Disable();
            _toggleTimeScaleAction.Disable();

            for (int i = _timers.Count - 1; i >= 0; i--)
            {
                Timer t = _timers[i];
                if (t == null) continue;
                if (t.IsDone || t.IsCancelled) continue;
                t.Cancel();
            }
            _timers.Clear();
            _lastTimer = null;
        }

        private void OnDestroy()
        {
            _registerScaledDelayAction.performed -= OnRegisterScaledDelayPerformed;
            _registerUnscaledDelayAction.performed -= OnRegisterUnscaledDelayPerformed;
            _registerLoopAction.performed -= OnRegisterLoopPerformed;
            _togglePauseLastAction.performed -= OnTogglePauseLastPerformed;
            _cancelLastAction.performed -= OnCancelLastPerformed;
            _clearAllAction.performed -= OnClearAllPerformed;
            _toggleTimeScaleAction.performed -= OnToggleTimeScalePerformed;

            _registerScaledDelayAction.Dispose();
            _registerUnscaledDelayAction.Dispose();
            _registerLoopAction.Dispose();
            _togglePauseLastAction.Dispose();
            _cancelLastAction.Dispose();
            _clearAllAction.Dispose();
            _toggleTimeScaleAction.Dispose();
        }

        private static InputAction NewAction(string name, string binding)
        {
            return new InputAction(
                name: name,
                type: InputActionType.Button,
                binding: binding
            );
        }

        private void OnRegisterScaledDelayPerformed(InputAction.CallbackContext _)
        {
            RegisterScaledDelay();
        }

        private void OnRegisterUnscaledDelayPerformed(InputAction.CallbackContext _)
        {
            RegisterUnscaledDelay();
        }

        private void OnRegisterLoopPerformed(InputAction.CallbackContext _)
        {
            RegisterLoop();
        }

        private void OnTogglePauseLastPerformed(InputAction.CallbackContext _)
        {
            TogglePauseLast();
        }

        private void OnCancelLastPerformed(InputAction.CallbackContext _)
        {
            CancelLast();
        }

        private void OnClearAllPerformed(InputAction.CallbackContext _)
        {
            ClearAll();
        }

        private void OnToggleTimeScalePerformed(InputAction.CallbackContext _)
        {
            ToggleTimeScale();
        }

        private void RegisterScaledDelay()
        {
            TimerManager mgr = TimerManager.Instance;
            if (mgr == null)
            {
                LogError("TimerManager.Instance is null (did singleton fail to init?)");
                return;
            }

            float seconds = Mathf.Max(0f, _scaledDelaySeconds);
            Log($"Register scaled delay: {seconds:0.###}s");

            Timer timer = null;
            timer = mgr.Register(
                duration: seconds,
                onComplete: () =>
                {
                    try
                    {
                        Log($"Scaled delay complete ({seconds:0.###}s).");
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                },
                isLooping: false,
                useUnscaledTime: false,
                onUpdate: progress =>
                {
                    if (_lastTimer == null) return;
                    if (_lastTimer != timer) return;
                    // HUD reads progress via Timer fields; keep this callback for demo parity.
                }
            );

            Track(timer);
        }

        private void RegisterUnscaledDelay()
        {
            TimerManager mgr = TimerManager.Instance;
            if (mgr == null)
            {
                LogError("TimerManager.Instance is null (did singleton fail to init?)");
                return;
            }

            float seconds = Mathf.Max(0f, _unscaledDelaySeconds);
            Log($"Register unscaled delay: {seconds:0.###}s (ignores timeScale)");

            Timer timer = mgr.Register(
                duration: seconds,
                onComplete: () =>
                {
                    try
                    {
                        Log($"Unscaled delay complete ({seconds:0.###}s).");
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                },
                isLooping: false,
                useUnscaledTime: true
            );

            Track(timer);
        }

        private void RegisterLoop()
        {
            TimerManager mgr = TimerManager.Instance;
            if (mgr == null)
            {
                LogError("TimerManager.Instance is null (did singleton fail to init?)");
                return;
            }

            float seconds = Mathf.Max(0f, _loopIntervalSeconds);
            _loopTickCount = 0;
            Log($"Register looping timer: every {seconds:0.###}s (scaled)");

            Timer timer = mgr.Register(
                duration: seconds,
                onComplete: () =>
                {
                    try
                    {
                        _loopTickCount++;
                        Log($"Loop tick #{_loopTickCount} (interval {seconds:0.###}s).");
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                },
                isLooping: true,
                useUnscaledTime: false
            );

            Track(timer);
        }

        private void TogglePauseLast()
        {
            if (_lastTimer == null)
            {
                Log("No last timer to pause/resume.");
                return;
            }

            if (_lastTimer.IsCancelled || _lastTimer.IsDone)
            {
                Log("Last timer is already done/cancelled.");
                return;
            }

            if (_lastTimer.IsPaused)
            {
                _lastTimer.Resume();
                Log("Last timer resumed.");
            }
            else
            {
                _lastTimer.Pause();
                Log("Last timer paused.");
            }
        }

        private void CancelLast()
        {
            if (_lastTimer == null)
            {
                Log("No last timer to cancel.");
                return;
            }

            if (_lastTimer.IsCancelled || _lastTimer.IsDone)
            {
                Log("Last timer is already done/cancelled.");
                return;
            }

            _lastTimer.Cancel();
            Log("Last timer cancelled (will be recycled by manager).");
        }

        private void ClearAll()
        {
            TimerManager mgr = TimerManager.Instance;
            if (mgr == null)
            {
                LogError("TimerManager.Instance is null (did singleton fail to init?)");
                return;
            }

            Log("ClearAllTimers()");
            mgr.ClearAllTimers();

            _timers.Clear();
            _lastTimer = null;
        }

        private void ToggleTimeScale()
        {
            Time.timeScale = Mathf.Approximately(Time.timeScale, 0f) ? 1f : 0f;
            Log($"Time.timeScale -> {Time.timeScale:0.###}");
        }

        private void Track(Timer timer)
        {
            if (timer == null)
            {
                LogError("Register returned null timer (unexpected).");
                return;
            }

            _timers.Add(timer);
            _lastTimer = timer;
        }

        private void SweepDeadHandles()
        {
            for (int i = _timers.Count - 1; i >= 0; i--)
            {
                Timer t = _timers[i];
                if (t == null || t.IsCancelled || t.IsDone)
                {
                    _timers.RemoveAt(i);
                }
            }
        }

        private void Log(string message)
        {
            string line = $"[{Time.unscaledTime:0.000}] {message}";
            Debug.Log(line, this);

            _logLines.Add(line);
            while (_logLines.Count > Mathf.Max(1, _maxLogLines))
            {
                _logLines.RemoveAt(0);
            }
        }

        private void LogError(string message)
        {
            string line = $"[{Time.unscaledTime:0.000}] ERROR: {message}";
            Debug.LogError(line, this);

            _logLines.Add(line);
            while (_logLines.Count > Mathf.Max(1, _maxLogLines))
            {
                _logLines.RemoveAt(0);
            }
        }
    }
}

