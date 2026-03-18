namespace EW_Framework.Core.StateMachine.Base {
    /// <summary>
    /// Generic state interface for state machine
    /// T: The owner of the state machine (context environment)
    /// </summary>
    public interface IState<T>
    {
        /// <summary>
        /// Triggered once when entering this state
        /// </summary>
        /// <param name="context">The owner of the state machine</param>
        void Enter(T context);

        /// <summary>
        /// Triggered every frame (usually driven by the owner's Update)
        /// </summary>
        /// <param name="context">The owner of the state machine</param>
        void Update(T context);

        /// <summary>
        /// Triggered once when leaving this state
        /// </summary>
        /// <param name="context">The owner of the state machine</param>
        void Exit(T context);

        // Optional extension: you can add FixedUpdate(T context) for physical logic,
        // or LateUpdate(T context) for camera following logic.
    }

    /// <summary>
    /// Optional extension interface for stack-based state machines.
    /// Implement this if the state expects to be paused/resumed via PushState/PopState.
    /// </summary>
    public interface IStackState<T> : IState<T>
    {
        /// <summary>
        /// Called when another state is pushed on top of this state.
        /// </summary>
        void OnPause(T context);

        /// <summary>
        /// Called when this state becomes the active state again after PopState.
        /// </summary>
        void OnResume(T context);
    }
}