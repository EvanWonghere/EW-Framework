using UnityEngine;
using UnityEngine.InputSystem;

namespace EW_Framework.Core.StateMachine.Examples
{
    /// <summary>
    /// Minimal controllable target for the state machine to chase.
    /// Uses the new Input System (WASD / Arrow keys).
    /// </summary>
    public sealed class StateMachineExampleTarget : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _moveSpeed = 3.5f;

        private InputAction _moveAction;

        private void Awake()
        {
            _moveAction = new InputAction(
                name: "Move",
                type: InputActionType.Value,
                binding: "2DVector"
            );
            _moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");

            _moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/rightArrow");
        }

        private void OnEnable()
        {
            _moveAction.Enable();
        }

        private void Update()
        {
            Vector2 move = _moveAction.ReadValue<Vector2>();
            Vector3 delta = new Vector3(move.x, 0f, move.y) * (_moveSpeed * Time.deltaTime);
            transform.position += delta;
        }

        private void OnDisable()
        {
            _moveAction.Disable();
        }

        private void OnDestroy()
        {
            _moveAction.Dispose();
        }
    }
}

