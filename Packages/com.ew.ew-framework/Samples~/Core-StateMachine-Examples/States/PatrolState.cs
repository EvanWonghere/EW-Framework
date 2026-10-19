using EW_Framework.Core.StateMachine.Base;
using UnityEngine;

namespace EW_Framework.Core.StateMachine.Examples.States
{
    public sealed class PatrolState : IState<StateMachineExampleDriver>
    {
        private float _angle;

        public void Enter(StateMachineExampleDriver context)
        {
            _angle = 0f;
        }

        public void Update(StateMachineExampleDriver context)
        {
            // Simple patrol: move in a small circle
            _angle += Time.deltaTime * 1.2f;
            float dx = Mathf.Cos(_angle) * 0.5f * Time.deltaTime;
            float dz = Mathf.Sin(_angle) * 0.5f * Time.deltaTime;
            context.Move(new Vector3(dx, 0f, dz));

            if (context.DistanceToTarget() <= context.ChaseEnterRange)
            {
                context.ChangeState<ChaseState>("目标进入追逐范围");
            }
        }

        public void Exit(StateMachineExampleDriver context)
        {
        }
    }
}

