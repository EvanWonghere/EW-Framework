using System.Collections.Generic;
using NUnit.Framework;
using EW_Framework.Core.TimerSystem;

namespace EW_Framework.Tests.Editor
{
    /// <summary>
    /// Drives <see cref="Timer"/> directly through Tick(deltaTime), so no PlayMode or TimerManager is needed.
    /// </summary>
    public class TimerTests
    {
        private int _completeCount;
        private Timer _timer;

        [SetUp]
        public void SetUp()
        {
            _completeCount = 0;
            _timer = new Timer();
        }

        [Test]
        public void Delay_CompletesOnceWhenDurationIsReached()
        {
            _timer.Initialize(1f, () => _completeCount++, isLooping: false, useUnscaledTime: false);

            _timer.Tick(0.5f);
            Assert.AreEqual(0, _completeCount);
            Assert.IsFalse(_timer.IsDone);

            _timer.Tick(0.5f);
            Assert.AreEqual(1, _completeCount);
            Assert.IsTrue(_timer.IsDone);

            _timer.Tick(1f);
            Assert.AreEqual(1, _completeCount);
        }

        [Test]
        public void Loop_FiresEveryDurationAndCarriesOverRemainder()
        {
            _timer.Initialize(1f, () => _completeCount++, isLooping: true, useUnscaledTime: false);

            _timer.Tick(1.25f);
            Assert.AreEqual(1, _completeCount);
            Assert.AreEqual(0.25f, _timer.TimeElapsed, 1e-5f);
            Assert.IsFalse(_timer.IsDone);

            _timer.Tick(0.75f);
            Assert.AreEqual(2, _completeCount);

            _timer.Tick(1f);
            Assert.AreEqual(3, _completeCount);
        }

        [Test]
        public void Cancel_StopsFurtherCallbacks()
        {
            _timer.Initialize(1f, () => _completeCount++, isLooping: true, useUnscaledTime: false);
            _timer.Tick(0.5f);

            _timer.Cancel();
            _timer.Tick(1f);

            Assert.IsTrue(_timer.IsCancelled);
            Assert.AreEqual(0, _completeCount);
        }

        [Test]
        public void Pause_FreezesElapsedTimeUntilResume()
        {
            _timer.Initialize(1f, () => _completeCount++, isLooping: false, useUnscaledTime: false);

            _timer.Pause();
            _timer.Tick(5f);
            Assert.AreEqual(0f, _timer.TimeElapsed);
            Assert.AreEqual(0, _completeCount);

            _timer.Resume();
            _timer.Tick(1f);
            Assert.AreEqual(1, _completeCount);
        }

        [Test]
        public void OnUpdate_ReportsProgressClampedToOne()
        {
            var progress = new List<float>();
            _timer.Initialize(2f, null, isLooping: false, useUnscaledTime: false, onUpdate: progress.Add);

            _timer.Tick(0.5f);
            _timer.Tick(3f);

            Assert.AreEqual(2, progress.Count);
            Assert.AreEqual(0.25f, progress[0], 1e-5f);
            Assert.AreEqual(1f, progress[1], 1e-5f);
        }

        [Test]
        public void OnReturnPool_ResetsStateAndDropsCallbacks()
        {
            _timer.Initialize(1f, () => _completeCount++, isLooping: false, useUnscaledTime: false);
            _timer.Tick(1f);
            Assert.IsTrue(_timer.IsDone);

            _timer.OnReturnPool();

            Assert.IsFalse(_timer.IsDone);
            Assert.AreEqual(0f, _timer.TimeElapsed);
            // A recycled timer must not call back into its previous owner.
            _timer.Tick(1f);
            Assert.AreEqual(1, _completeCount);
        }
    }
}
