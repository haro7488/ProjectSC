using NUnit.Framework;
using Sc.Common;
using UnityEngine;

namespace Sc.Editor.Tests.Common
{
    /// <summary>
    /// ObjectPool 단위 테스트
    /// </summary>
    [TestFixture]
    public class ObjectPoolTests
    {
        private GameObject _containerObject;
        private Transform _container;
        private GameObject _prefabObject;
        private MockPoolable _prefab;
        private ObjectPool<MockPoolable> _pool;

        [SetUp]
        public void SetUp()
        {
            _containerObject = new GameObject("TestContainer");
            _container = _containerObject.transform;

            _prefabObject = new GameObject("TestPrefab");
            _prefab = _prefabObject.AddComponent<MockPoolable>();
        }

        [TearDown]
        public void TearDown()
        {
            _pool?.Clear();

            if (_containerObject != null)
                Object.DestroyImmediate(_containerObject);
            if (_prefabObject != null)
                Object.DestroyImmediate(_prefabObject);
        }

        #region Constructor Tests

        [Test]
        public void Constructor_WithInitialSize_PrewarmsPool()
        {
            _pool = new ObjectPool<MockPoolable>(_prefab, _container, initialSize: 5);

            Assert.That(_pool.PooledCount, Is.EqualTo(5));
            Assert.That(_pool.ActiveCount, Is.EqualTo(0));
            Assert.That(_pool.TotalCount, Is.EqualTo(5));
        }

        [Test]
        public void Constructor_WithZeroInitialSize_CreatesEmptyPool()
        {
            _pool = new ObjectPool<MockPoolable>(_prefab, _container, initialSize: 0);

            Assert.That(_pool.PooledCount, Is.EqualTo(0));
            Assert.That(_pool.TotalCount, Is.EqualTo(0));
        }

        #endregion

        #region Spawn Tests

        [Test]
        public void Spawn_ReturnsInstance()
        {
            _pool = new ObjectPool<MockPoolable>(_prefab, _container);

            var instance = _pool.Spawn();

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.gameObject.activeSelf, Is.True);
        }

        [Test]
        public void Spawn_CallsOnSpawn()
        {
            _pool = new ObjectPool<MockPoolable>(_prefab, _container);

            var instance = _pool.Spawn();

            Assert.That(instance.SpawnCount, Is.EqualTo(1));
        }

        [Test]
        public void Spawn_FromPrewarmedPool_ReusesInstance()
        {
            _pool = new ObjectPool<MockPoolable>(_prefab, _container, initialSize: 3);

            var instance = _pool.Spawn();

            Assert.That(_pool.PooledCount, Is.EqualTo(2));
            Assert.That(_pool.ActiveCount, Is.EqualTo(1));
        }

        [Test]
        public void Spawn_WithPosition_SetsPosition()
        {
            _pool = new ObjectPool<MockPoolable>(_prefab, _container);
            var position = new Vector3(1, 2, 3);

            var instance = _pool.Spawn(position);

            Assert.That(instance.transform.position, Is.EqualTo(position));
        }

        [Test]
        public void Spawn_WithPositionAndRotation_SetsBoth()
        {
            _pool = new ObjectPool<MockPoolable>(_prefab, _container);
            var position = new Vector3(1, 2, 3);
            var rotation = Quaternion.Euler(0, 90, 0);

            var instance = _pool.Spawn(position, rotation);

            Assert.That(instance.transform.position, Is.EqualTo(position));
            Assert.That(instance.transform.rotation.IsApproximately(rotation), Is.True);
        }

        [Test]
        public void Spawn_WhenPoolFull_ReturnsNull()
        {
            _pool = new ObjectPool<MockPoolable>(_prefab, _container, initialSize: 0, maxSize: 2);

            _pool.Spawn();
            _pool.Spawn();
            var third = _pool.Spawn();

            Assert.That(third, Is.Null);
        }

        [Test]
        public void Spawn_WithUnlimitedMaxSize_AlwaysCreates()
        {
            _pool = new ObjectPool<MockPoolable>(_prefab, _container, initialSize: 0, maxSize: 0);

            for (int i = 0; i < 100; i++)
            {
                var instance = _pool.Spawn();
                Assert.That(instance, Is.Not.Null);
            }

            Assert.That(_pool.ActiveCount, Is.EqualTo(100));
        }

        #endregion

        #region Despawn Tests

        [Test]
        public void Despawn_ReturnsToPool()
        {
            _pool = new ObjectPool<MockPoolable>(_prefab, _container);
            var instance = _pool.Spawn();

            _pool.Despawn(instance);

            Assert.That(_pool.PooledCount, Is.EqualTo(1));
            Assert.That(_pool.ActiveCount, Is.EqualTo(0));
            Assert.That(instance.gameObject.activeSelf, Is.False);
        }

        [Test]
        public void Despawn_CallsOnDespawn()
        {
            _pool = new ObjectPool<MockPoolable>(_prefab, _container);
            var instance = _pool.Spawn();

            _pool.Despawn(instance);

            Assert.That(instance.DespawnCount, Is.EqualTo(1));
        }

        [Test]
        public void Despawn_ReparentsToContainer()
        {
            _pool = new ObjectPool<MockPoolable>(_prefab, _container);
            var instance = _pool.Spawn();
            instance.transform.SetParent(null);

            _pool.Despawn(instance);

            Assert.That(instance.transform.parent, Is.EqualTo(_container));
        }

        [Test]
        public void Despawn_NullInstance_DoesNotThrow()
        {
            _pool = new ObjectPool<MockPoolable>(_prefab, _container);

            Assert.DoesNotThrow(() => _pool.Despawn(null));
        }

        #endregion

        #region DespawnAll Tests

        [Test]
        public void DespawnAll_ReturnsAllActiveToPool()
        {
            _pool = new ObjectPool<MockPoolable>(_prefab, _container);
            _pool.Spawn();
            _pool.Spawn();
            _pool.Spawn();

            _pool.DespawnAll();

            Assert.That(_pool.PooledCount, Is.EqualTo(3));
            Assert.That(_pool.ActiveCount, Is.EqualTo(0));
        }

        [Test]
        public void DespawnAll_CallsOnDespawnForAll()
        {
            _pool = new ObjectPool<MockPoolable>(_prefab, _container);
            var instances = new[]
            {
                _pool.Spawn(),
                _pool.Spawn(),
                _pool.Spawn()
            };

            _pool.DespawnAll();

            foreach (var instance in instances)
            {
                Assert.That(instance.DespawnCount, Is.EqualTo(1));
            }
        }

        #endregion

        #region Clear Tests

        [Test]
        public void Clear_DestroysAllInstances()
        {
            _pool = new ObjectPool<MockPoolable>(_prefab, _container, initialSize: 3);
            _pool.Spawn();

            _pool.Clear();

            Assert.That(_pool.PooledCount, Is.EqualTo(0));
            Assert.That(_pool.ActiveCount, Is.EqualTo(0));
            Assert.That(_pool.TotalCount, Is.EqualTo(0));
        }

        #endregion

        #region Reuse Tests

        [Test]
        public void SpawnDespawnSpawn_ReusesSameInstance()
        {
            _pool = new ObjectPool<MockPoolable>(_prefab, _container);

            var first = _pool.Spawn();
            _pool.Despawn(first);
            var second = _pool.Spawn();

            Assert.That(second, Is.SameAs(first));
            Assert.That(first.SpawnCount, Is.EqualTo(2));
        }

        #endregion
    }

    /// <summary>
    /// 테스트용 IPoolable 구현
    /// </summary>
    public class MockPoolable : MonoBehaviour, IPoolable
    {
        public int SpawnCount { get; private set; }
        public int DespawnCount { get; private set; }

        public void OnSpawn()
        {
            SpawnCount++;
        }

        public void OnDespawn()
        {
            DespawnCount++;
        }
    }

    /// <summary>
    /// Quaternion 비교 확장
    /// </summary>
    public static class QuaternionExtensions
    {
        public static bool IsApproximately(this Quaternion a, Quaternion b, float tolerance = 0.0001f)
        {
            return Quaternion.Dot(a, b) > 1 - tolerance;
        }
    }
}
