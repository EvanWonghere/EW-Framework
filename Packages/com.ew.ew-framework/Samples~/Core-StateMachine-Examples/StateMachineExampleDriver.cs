using EW_Framework.Core.StateMachine.Base;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EW_Framework.Core.StateMachine.Examples
{
    public sealed class StateMachineExampleDriver : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _target;

        [Header("Ranges")]
        [SerializeField, Min(0f)] private float _chaseEnterRange = 5f;
        [SerializeField, Min(0f)] private float _chaseLoseRange = 7f;

        [Header("Timings")]
        [SerializeField, Min(0f)] private float _idleDuration = 1f;
        [SerializeField, Min(0f)] private float _chaseTimeout = 6f;

        private StateMachine<StateMachineExampleDriver> _fsm;
        private InputAction _togglePauseAction;

        private string _lastTransition;
        private float _lastTransitionTime;

        public Transform Target => _target;
        public float ChaseEnterRange => _chaseEnterRange;
        public float ChaseLoseRange => _chaseLoseRange;
        public float IdleDuration => _idleDuration;
        public float ChaseTimeout => _chaseTimeout;

        public Vector3 Position => transform.position;
        public string CurrentStateName => _fsm?.CurrentState?.GetType().Name ?? "<None>";
        public string LastTransition => string.IsNullOrWhiteSpace(_lastTransition) ? "<None>" : _lastTransition;
        public float LastTransitionTime => _lastTransitionTime;

        private void Awake()
        {
            _fsm = new StateMachine<StateMachineExampleDriver>(this);
            _togglePauseAction = new InputAction(
                name: "TogglePause",
                type: InputActionType.Button,
                binding: "<Keyboard>/escape"
            );
            _togglePauseAction.performed += OnTogglePausePerformed;
        }

        private void OnEnable()
        {
            _fsm.Start<States.IdleState>();
            _togglePauseAction.Enable();
        }

        private void Update()
        {
            _fsm.Update();
        }

        private void OnDisable()
        {
            _togglePauseAction.Disable();
            _fsm.Clear();
        }

        private void OnDestroy()
        {
            _togglePauseAction.performed -= OnTogglePausePerformed;
            _togglePauseAction.Dispose();
        }

        private void OnValidate()
        {
            if (_chaseLoseRange < _chaseEnterRange)
            {
                _chaseLoseRange = _chaseEnterRange;
            }
        }

        public float DistanceToTarget()
        {
            if (_target == null) return float.PositiveInfinity;
            return Vector3.Distance(transform.position, _target.position);
        }

        public void StartFsm<TState>() where TState : class, IState<StateMachineExampleDriver>, new()
        {
            _fsm.Start<TState>();
        }

        public void ChangeState<TState>() where TState : class, IState<StateMachineExampleDriver>, new()
        {
            ChangeState<TState>(reason: null);
        }

        public void ChangeState<TState>(string reason) where TState : class, IState<StateMachineExampleDriver>, new()
        {
            string from = _fsm.CurrentState?.GetType().Name ?? "<None>";
            _fsm.ChangeState<TState>();
            string to = _fsm.CurrentState?.GetType().Name ?? typeof(TState).Name;
            RecordTransition($"ChangeState: {from} -> {to}", reason);
        }

        public void PushState<TState>() where TState : class, IState<StateMachineExampleDriver>, new()
        {
            string from = _fsm.CurrentState?.GetType().Name ?? "<None>";
            _fsm.PushState<TState>();
            string to = _fsm.CurrentState?.GetType().Name ?? typeof(TState).Name;
            RecordTransition($"PushState: {from} -> {to}", reason: null);
        }

        public void PopState()
        {
            string from = _fsm.CurrentState?.GetType().Name ?? "<None>";
            _fsm.PopState();
            string to = _fsm.CurrentState?.GetType().Name ?? "<None>";
            RecordTransition($"PopState: {from} -> {to}", reason: null);
        }

        public void Move(Vector3 delta)
        {
            transform.position += delta;
        }

        private void OnTogglePausePerformed(InputAction.CallbackContext _)
        {
            if (_fsm.CurrentState is States.PauseState)
            {
                PopState();
            }
            else
            {
                PushState<States.PauseState>();
            }
        }

        private void RecordTransition(string transition, string reason)
        {
            _lastTransitionTime = Time.time;
            _lastTransition = string.IsNullOrWhiteSpace(reason) ? transition : $"{transition} ({reason})";
        }
    }
}

