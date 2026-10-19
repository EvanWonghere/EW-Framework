using EW_Framework.Core.StateMachine.Base;
using UnityEngine;

namespace EW_Framework.Core.StateMachine.Examples.States
{
    public sealed class PauseState : IStackState<StateMachineExampleDriver>
    {
        public void Enter(StateMachineExampleDriver context)
        {
            Debug.Log("[StateMachineExample] Paused (PushState). Press Esc to resume (PopState).");
        }

        public void Update(StateMachineExampleDriver context)
        {
            // Intentionally do nothing while paused.
        }

        public void Exit(StateMachineExampleDriver context)
        {
            Debug.Log("[StateMachineExample] PauseState exited.");
        }

        public void OnPause(StateMachineExampleDriver context)
        {
            // PauseState itself is never paused in this example.
        }

        public void OnResume(StateMachineExampleDriver context)
        {
            // PauseState itself is never resumed in this example.
        }
    }
}

