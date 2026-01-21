using NUnit.Framework;
using Sc.Common;
using UnityEngine;

namespace Sc.Editor.Tests.Common
{
    /// <summary>
    /// PoolManager 단위 테스트
    /// </summary>
    [TestFixture]
    public class PoolManagerTests
    {
        private PoolManager _manager;
        private GameObject _managerObject;
        private GameObject _prefabObject;
        private MockPoolable _prefab;

        [SetUp]
        public void SetUp()
        {
            _managerObject = new GameObject("TestPoolManager");
            _manager = _managerObject.AddComponent<PoolManager>();

            _prefabObject = new GameObject("TestPrefab");
            _prefab = _prefabObject.AddComponent<MockPoolable>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_manager != null)
            {
                _manager.ClearAllPools();
            }

            if (_managerObject != null)
                Object.DestroyImmediate(_managerObject);
            if (_prefabObject != null)
                Object.DestroyImmediate(_prefabObject);
        }

        #region CreatePool Tests

        [Test]
        public void CreatePool_CreatesNewPool()
        {
            _manager.CreatePool(_prefab, initialSize: 5, maxSize: 10);

            Assert.That(_manager.HasPool<MockPoolable>(), Is.True);
        }

        [Test]
        public void CreatePool_DuplicateCall_DoesNotThrow()
        {
            _manager.CreatePool(_prefab);

            Assert.DoesNotThrow(() => _manager.CreatePool(_prefab));
        }

        #endregion

        #region Spawn Tests

        [Test]
        public void Spawn_WithoutPool_ReturnsNull()
        {
            var instance = _manager.Spawn<MockPoolable>();

            Assert.That(instance, Is.Null);
        }

        [Test]
        public void Spawn_WithPool_ReturnsInstance()
        {
            _manager.CreatePool(_prefab);

            var instance = _manager.Spawn<MockPoolable>();

            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void Spawn_WithPosition_SetsPosition()
        {
            _manager.CreatePool(_prefab);
            var position = new Vector3(5, 10, 15);

            var instance = _manager.Spawn<MockPoolable>(position);

            Assert.That(instance.transform.position, Is.EqualTo(position));
        }

        [Test]
        public void Spawn_WithPositionAndRotation_SetsBoth()
        {
            _manager.CreatePool(_prefab);
            var position = new Vector3(5, 10, 15);
            var rotation = Quaternion.Euler(45, 90, 0);

            var instance = _manager.Spawn<MockPoolable>(position, rotation);

            Assert.That(instance.transform.position, Is.EqualTo(position));
            Assert.That(instance.transform.rotation.IsApproximately(rotation), Is.True);
        }

        #endregion

        #region Despawn Tests

        [Test]
        public void Despawn_ReturnsToPool()
        {
            _manager.CreatePool(_prefab);
            var instance = _manager.Spawn<MockPoolable>();

            _manager.Despawn(instance);

            var (pooled, active) = _manager.GetStats<MockPoolable>();
            Assert.That(pooled, Is.EqualTo(1));
            Assert.That(active, Is.EqualTo(0));
        }

        [Test]
        public void DespawnAll_ReturnsAllActive()
        {
            _manager.CreatePool(_prefab);
            _manager.Spawn<MockPoolable>();
            _manager.Spawn<MockPoolable>();
            _manager.Spawn<MockPoolable>();

            _manager.DespawnAll<MockPoolable>();

            var (pooled, active) = _manager.GetStats<MockPoolable>();
            Assert.That(pooled, Is.EqualTo(3));
            Assert.That(active, Is.EqualTo(0));
        }

        #endregion

        #region ClearPool Tests

        [Test]
        public void ClearPool_RemovesPool()
        {
            _manager.CreatePool(_prefab);
            _manager.Spawn<MockPoolable>();

            _manager.ClearPool<MockPoolable>();

            Assert.That(_manager.HasPool<MockPoolable>(), Is.False);
        }

        [Test]
        public void ClearAllPools_RemovesAllPools()
        {
            _manager.CreatePool(_prefab);

            _manager.ClearAllPools();

            Assert.That(_manager.HasPool<MockPoolable>(), Is.False);
        }

        #endregion

        #region GetStats Tests

        [Test]
        public void GetStats_ReturnsCorrectCounts()
        {
            _manager.CreatePool(_prefab, initialSize: 5);
            _manager.Spawn<MockPoolable>();
            _manager.Spawn<MockPoolable>();

            var (pooled, active) = _manager.GetStats<MockPoolable>();

            Assert.That(pooled, Is.EqualTo(3));
            Assert.That(active, Is.EqualTo(2));
        }

        [Test]
        public void GetStats_WithoutPool_ReturnsZero()
        {
            var (pooled, active) = _manager.GetStats<MockPoolable>();

            Assert.That(pooled, Is.EqualTo(0));
            Assert.That(active, Is.EqualTo(0));
        }

        #endregion
    }
}
