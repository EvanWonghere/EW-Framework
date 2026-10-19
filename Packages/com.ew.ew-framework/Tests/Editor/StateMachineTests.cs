using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using EW_Framework.Core.StateMachine.Base;

namespace EW_Framework.Tests.Editor
{
    public class StateMachineTests
    {
        private class Owner
        {
            public readonly List<string> Log = new();
        }

        // Supports Pause/Resume, so it can sit under a pushed state.
        private class StateA : IStackState<Owner>
        {
            public void Enter(Owner context) => context.Log.Add("A.Enter");
            public void Update(Owner context) => context.Log.Add("A.Update");
            public void Exit(Owner context) => context.Log.Add("A.Exit");
            public void OnPause(Owner context) => context.Log.Add("A.Pause");
            public void OnResume(Owner context) => context.Log.Add("A.Resume");
        }

        private class StateB : IState<Owner>
        {
            public void Enter(Owner context) => context.Log.Add("B.Enter");
            public void Update(Owner context) => context.Log.Add("B.Update");
            public void Exit(Owner context) => context.Log.Add("B.Exit");
        }

        private Owner _owner;
        private StateMachine<Owner> _machine;

        [SetUp]
        public void SetUp()
        {
            _owner = new Owner();
            _machine = new StateMachine<Owner>(_owner);
        }

        [Test]
        public void Start_EntersInitialState()
        {
            _machine.Start<StateA>();

            Assert.IsInstanceOf<StateA>(_machine.CurrentState);
            CollectionAssert.AreEqual(new[] { "A.Enter" }, _owner.Log);
        }

        [Test]
        public void ChangeState_ExitsCurrentBeforeEnteringNext()
        {
            _machine.Start<StateA>();
            _machine.ChangeState<StateB>();

            Assert.IsInstanceOf<StateB>(_machine.CurrentState);
            CollectionAssert.AreEqual(new[] { "A.Enter", "A.Exit", "B.Enter" }, _owner.Log);
        }

        [Test]
        public void ChangeState_ToCurrentStateType_IsIgnored()
        {
            _machine.Start<StateA>();
            _machine.ChangeState<StateA>();

            CollectionAssert.AreEqual(new[] { "A.Enter" }, _owner.Log);
        }

        [Test]
        public void ChangeState_ReusesCachedInstancePerType()
        {
            _machine.Start<StateA>();
            IState<Owner> first = _machine.CurrentState;

            _machine.ChangeState<StateB>();
            _machine.ChangeState<StateA>();

            Assert.AreSame(first, _machine.CurrentState);
        }

        [Test]
        public void PushState_PausesPreviousStateWithoutExitingIt()
        {
            _machine.Start<StateA>();
            _machine.PushState<StateB>();

            Assert.IsInstanceOf<StateB>(_machine.CurrentState);
            CollectionAssert.AreEqual(new[] { "A.Enter", "A.Pause", "B.Enter" }, _owner.Log);
        }

        [Test]
        public void PopState_ExitsTopAndResumesPreviousWithoutReEntering()
        {
            _machine.Start<StateA>();
            _machine.PushState<StateB>();
            _machine.PopState();

            Assert.IsInstanceOf<StateA>(_machine.CurrentState);
            CollectionAssert.AreEqual(
                new[] { "A.Enter", "A.Pause", "B.Enter", "B.Exit", "A.Resume" },
                _owner.Log);
        }

        [Test]
        public void PopState_WithEmptyStack_LogsWarningAndKeepsCurrentState()
        {
            _machine.Start<StateA>();

            LogAssert.Expect(LogType.Warning, "[StateMachine] State stack is empty. PopState ignored.");
            _machine.PopState();

            Assert.IsInstanceOf<StateA>(_machine.CurrentState);
            CollectionAssert.AreEqual(new[] { "A.Enter" }, _owner.Log);
        }

        [Test]
        public void Constructor_WithNullContext_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new StateMachine<Owner>(null));
        }
    }
}
