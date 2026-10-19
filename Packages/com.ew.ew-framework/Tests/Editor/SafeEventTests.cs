using System;
using NUnit.Framework;
using EW_Framework.Core.SOEventBus.Base;

namespace EW_Framework.Tests.Editor
{
    /// <summary>
    /// SafeEvent is the kernel behind every GameEventSO channel.
    /// </summary>
    public class SafeEventTests
    {
        [Test]
        public void Register_SameListenerTwice_IsInvokedOnce()
        {
            var safeEvent = new SafeEvent<int>();
            int calls = 0;
            Action<int> listener = _ => calls++;

            safeEvent.Register(listener);
            safeEvent.Register(listener);
            safeEvent.Raise(1);

            Assert.AreEqual(1, calls);
        }

        [Test]
        public void Raise_ListenerUnregisteringItself_DoesNotBreakOtherListeners()
        {
            var safeEvent = new SafeEvent<int>();
            int otherCalls = 0;
            Action<int> selfRemoving = null;
            selfRemoving = _ => safeEvent.Unregister(selfRemoving);

            safeEvent.Register(selfRemoving);
            safeEvent.Register(_ => otherCalls++);

            Assert.DoesNotThrow(() => safeEvent.Raise(1));
            Assert.AreEqual(1, otherCalls);
            Assert.AreEqual(1, safeEvent.GetListeners().Count);
        }
    }
}
