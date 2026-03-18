using EW_Framework.Core.StateMachine.Base;
using UnityEngine;

namespace EW_Framework.Core.StateMachine.Examples.States
{
    public sealed class IdleState : IState<StateMachineExampleDriver>
    {
        private float _t;

        public void Enter(StateMachineExampleDriver context)
        {
            _t = 0f;
        }

        public void Update(StateMachineExampleDriver context)
        {
            _t += Time.deltaTime;
            if (_t >= context.IdleDuration)
            {
                context.ChangeState<PatrolState>("Idle 计时结束");
            }
        }

        public void Exit(StateMachineExampleDriver context)
        {
        }
    }
}

