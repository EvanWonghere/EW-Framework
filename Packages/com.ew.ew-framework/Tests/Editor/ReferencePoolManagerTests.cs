using NUnit.Framework;
using EW_Framework.Core.ObjectPool.Base;
using EW_Framework.Core.ObjectPool.Manager;

namespace EW_Framework.Tests.Editor
{
    public class ReferencePoolManagerTests
    {
        private class PooledItem : IReference<PooledItem>
        {
            public int Payload;
            // Deliberately not reset in OnReturnPool, so tests can observe how often it ran.
            public int ReturnCount;

            public void OnReturnPool()
            {
                Payload = 0;
                ReturnCount++;
            }
        }

        // The pool is static per type, so every test starts from an empty pool.
        [SetUp]
        public void SetUp() => ReferencePoolManager.Clear<PooledItem>();

        [TearDown]
        public void TearDown() => ReferencePoolManager.Clear<PooledItem>();

        [Test]
        public void Release_InvokesOnReturnPool()
        {
            var item = ReferencePoolManager.Acquire<PooledItem>();
            item.Payload = 42;

            ReferencePoolManager.Release(item);

            Assert.AreEqual(1, item.ReturnCount);
            Assert.AreEqual(0, item.Payload);
        }

        [Test]
        public void Acquire_AfterRelease_ReusesTheSameInstance()
        {
            var first = ReferencePoolManager.Acquire<PooledItem>();
            ReferencePoolManager.Release(first);

            var second = ReferencePoolManager.Acquire<PooledItem>();

            Assert.AreSame(first, second);
        }

        [Test]
        public void Acquire_WhileFirstIsInUse_ReturnsDistinctInstance()
        {
            var first = ReferencePoolManager.Acquire<PooledItem>();
            var second = ReferencePoolManager.Acquire<PooledItem>();

            Assert.AreNotSame(first, second);
        }

        [Test]
        public void Clear_DropsPooledInstances()
        {
            var first = ReferencePoolManager.Acquire<PooledItem>();
            ReferencePoolManager.Release(first);

            ReferencePoolManager.Clear<PooledItem>();
            var second = ReferencePoolManager.Acquire<PooledItem>();

            Assert.AreNotSame(first, second);
        }

        [Test]
        public void Release_Null_IsIgnored()
        {
            Assert.DoesNotThrow(() => ReferencePoolManager.Release<PooledItem>(null));
        }
    }
}
