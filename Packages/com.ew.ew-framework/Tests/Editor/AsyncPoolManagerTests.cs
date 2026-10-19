using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.TestTools;
using EW_Framework.Core.ObjectPool.Manager;

namespace EW_Framework.Tests.Editor
{
    public class AsyncPoolManagerTests
    {
        private GameObject _host;

        [TearDown]
        public void TearDown()
        {
            if (_host != null) UnityEngine.Object.DestroyImmediate(_host);
        }

        /// <summary>
        /// Two first-time spawns of the same GUID share one pool-creation task. The load fails (the key does not exist),
        /// but both callers must get null rather than an exception from awaiting the shared task twice.
        /// </summary>
        [UnityTest]
        public IEnumerator SpawnAsync_ConcurrentFirstSpawnsOfSameGuid_BothComplete() => UniTask.ToCoroutine(async () =>
        {
            _host = new GameObject("AsyncPoolManagerTests");
            var manager = _host.AddComponent<AsyncPoolManager>();
            var missing = new AssetReference("0123456789abcdef0123456789abcdef");
            LogAssert.ignoreFailingMessages = true;

            UniTask<GameObject> first = manager.SpawnAsync(missing, Vector3.zero, Quaternion.identity);
            // Only meaningful while the first creation is still loading; if it finished synchronously nothing is shared.
            Assume.That(first.Status, Is.EqualTo(UniTaskStatus.Pending));
            UniTask<GameObject> second = manager.SpawnAsync(missing, Vector3.zero, Quaternion.identity);

            Exception error = null;
            (GameObject, GameObject) results = default;
            try
            {
                results = await UniTask.WhenAll(first, second);
            }
            catch (Exception e)
            {
                error = e;
            }

            Assert.IsNull(error, error?.ToString());
            Assert.IsNull(results.Item1);
            Assert.IsNull(results.Item2);
        });
    }
}
