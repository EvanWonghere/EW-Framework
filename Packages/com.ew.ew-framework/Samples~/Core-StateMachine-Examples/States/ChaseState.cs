using EW_Framework.Core.StateMachine.Base;
using UnityEngine;

namespace EW_Framework.Core.StateMachine.Examples.States
{
    public sealed class ChaseState : IState<StateMachineExampleDriver>
    {
        private float _t;

        public void Enter(StateMachineExampleDriver context)
        {
            _t = 0f;
        }

        public void Update(StateMachineExampleDriver context)
        {
            _t += Time.deltaTime;

            Transform target = context.Target;
            if (target == null)
            {
                context.ChangeState<PatrolState>("目标引用为空");
                return;
            }

            Vector3 toTarget = target.position - context.Position;
            float distance = toTarget.magnitude;

            // Lose target -> back to patrol
            if (distance >= context.ChaseLoseRange)
            {
                context.ChangeState<PatrolState>("目标离开丢失范围");
                return;
            }

            // Timeout -> back to patrol (keeps demo deterministic)
            if (_t >= context.ChaseTimeout)
            {
                context.ChangeState<PatrolState>("追逐超时");
                return;
            }

            // Move towards target
            Vector3 dir = distance > 0.0001f ? (toTarget / distance) : Vector3.zero;
            const float speed = 2.0f;
            context.Move(dir * (speed * Time.deltaTime));
        }

        public void Exit(StateMachineExampleDriver context)
        {
        }
    }
}

