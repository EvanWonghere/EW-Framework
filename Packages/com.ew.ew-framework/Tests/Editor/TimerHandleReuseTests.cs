using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using EW_Framework.Core.TimerSystem;
using EW_Framework.Modules.AudioSystem.Runtime;

namespace EW_Framework.Tests.Editor
{
    /// <summary>
    /// TimerManager recycles finished timers through the reference pool, so the next Register can hand out the
    /// same instance. A component still holding the old handle must not be able to cancel the new owner's timer.
    /// In EditMode Awake/Update do not run on their own, so they are invoked through reflection.
    /// </summary>
    public class TimerHandleReuseTests
    {
        private GameObject _managerObject;
        private GameObject _playerObject;
        private TimerManager _manager;

        [SetUp]
        public void SetUp()
        {
            _managerObject = new GameObject("TimerManager");
            _manager = _managerObject.AddComponent<TimerManager>();
        }

        [TearDown]
        public void TearDown()
        {
            _manager.ClearAllTimers();
            Object.DestroyImmediate(_managerObject);
            if (_playerObject != null) Object.DestroyImmediate(_playerObject);
        }

        [Test]
        public void StaleHandle_CancelAfterTimerIsReused_DoesNotAffectNewOwner()
        {
            var stale = _manager.Register(1f, null);
            Invoke(_manager, "Update");
            stale.Cancel();
            Invoke(_manager, "Update"); // sweep: the cancelled timer goes back to the pool

            int fired = 0;
            var fresh = _manager.Register(0f, () => fired++);
            Assume.That(Underlying(fresh), Is.SameAs(Underlying(stale)), "the pool should hand the recycled instance to the next Register");

            stale.Cancel();
            stale.Pause();

            Assert.IsFalse(fresh.IsCancelled);
            Assert.IsFalse(fresh.IsPaused);
            Invoke(_manager, "Update");
            Assert.AreEqual(1, fired);
        }

        [Test]
        public void ClearAllTimersInsideCallback_ThenRegister_NewTimerIsNotMarkedDone()
        {
            var first = default(TimerHandle);
            var second = default(TimerHandle);
            first = _manager.Register(0f, () =>
            {
                _manager.ClearAllTimers();
                second = _manager.Register(10f, null);
            });

            Invoke(_manager, "Update");

            Assume.That(Underlying(second), Is.SameAs(Underlying(first)), "the pool should hand the recycled instance to the next Register");
            Assert.IsTrue(second.IsActive, "the timer registered from the callback was finished by the Tick that invoked it");
        }

        [Test]
        public void AudioPlayer_FinishedFadeTimer_DoesNotCancelReusedTimerOnDespawn()
        {
            _playerObject = new GameObject("AudioPlayer", typeof(AudioSource));
            var player = _playerObject.AddComponent<AudioPlayer>();
            Invoke(player, "Awake");

            var clip = AudioClip.Create("test", 4410, 1, 44100, false);
            player.ExecuteCommand(new AudioCommand
            {
                CmdType = AudioCommandType.Play,
                Clip = clip,
                Volume = 1f,
                Pitch = 1f,
                Loop = true,
                FadeDuration = 0.5f,
            });

            // Finish the fade-in and let the manager recycle it.
            object fade = typeof(AudioPlayer).GetField("_fadeTimer", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player);
            Timer fadeTimer = Underlying(fade);
            Assert.IsNotNull(fadeTimer);
            Invoke(_manager, "Update");
            fadeTimer.Tick(1f);
            Invoke(_manager, "Update");

            var other = _manager.Register(10f, null);
            Assume.That(Underlying(other), Is.SameAs(fadeTimer), "the pool should hand the recycled instance to the next Register");

            player.OnDespawn();

            Assert.IsFalse(other.IsCancelled, "AudioPlayer cancelled a timer it no longer owns");
            Object.DestroyImmediate(clip);
        }

        /// <summary>The pooled Timer behind a Register result, whatever type Register returns.</summary>
        private static Timer Underlying(object handle) =>
            handle as Timer ?? (Timer)handle.GetType().GetField("_timer", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(handle);

        private static void Invoke(object target, string method) =>
            target.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Invoke(target, null);
    }
}
