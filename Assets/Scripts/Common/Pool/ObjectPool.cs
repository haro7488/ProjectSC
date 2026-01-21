using System.Collections.Generic;
using UnityEngine;

namespace Sc.Common
{
    /// <summary>
    /// 제네릭 오브젝트 풀
    /// </summary>
    /// <typeparam name="T">Component이면서 IPoolable을 구현하는 타입</typeparam>
    public class ObjectPool<T> : IClearable where T : Component, IPoolable
    {
        private readonly T _prefab;
        private readonly Transform _container;
        private readonly int _maxSize;

        private readonly Stack<T> _pooled = new();
        private readonly HashSet<T> _active = new();

        /// <summary>
        /// 대기 중인 객체 수
        /// </summary>
        public int PooledCount => _pooled.Count;

        /// <summary>
        /// 사용 중인 객체 수
        /// </summary>
        public int ActiveCount => _active.Count;

        /// <summary>
        /// 총 객체 수 (대기 + 사용)
        /// </summary>
        public int TotalCount => PooledCount + ActiveCount;

        /// <summary>
        /// 풀 생성
        /// </summary>
        /// <param name="prefab">풀링할 프리팹</param>
        /// <param name="container">풀 컨테이너 Transform</param>
        /// <param name="initialSize">초기 생성 수</param>
        /// <param name="maxSize">최대 크기 (0 = 무제한)</param>
        public ObjectPool(T prefab, Transform container, int initialSize = 0, int maxSize = 0)
        {
            _prefab = prefab;
            _container = container;
            _maxSize = maxSize;

            // 초기 예열 (Prewarm)
            for (int i = 0; i < initialSize; i++)
            {
                var instance = CreateInstance();
                instance.gameObject.SetActive(false);
                _pooled.Push(instance);
            }
        }

        /// <summary>
        /// 풀에서 객체 꺼내기
        /// </summary>
        public T Spawn()
        {
            T instance;

            if (_pooled.Count > 0)
            {
                instance = _pooled.Pop();
            }
            else if (_maxSize <= 0 || TotalCount < _maxSize)
            {
                instance = CreateInstance();
            }
            else
            {
                Debug.LogWarning($"[ObjectPool] {typeof(T).Name} 풀이 가득 참 (max: {_maxSize})");
                return null;
            }

            instance.gameObject.SetActive(true);
            instance.OnSpawn();
            _active.Add(instance);

            return instance;
        }

        /// <summary>
        /// 위치 지정 스폰
        /// </summary>
        public T Spawn(Vector3 position)
        {
            var instance = Spawn();
            if (instance != null)
            {
                instance.transform.position = position;
            }
            return instance;
        }

        /// <summary>
        /// 위치 및 회전 지정 스폰
        /// </summary>
        public T Spawn(Vector3 position, Quaternion rotation)
        {
            var instance = Spawn();
            if (instance != null)
            {
                instance.transform.SetPositionAndRotation(position, rotation);
            }
            return instance;
        }

        /// <summary>
        /// 풀에 반환
        /// </summary>
        public void Despawn(T instance)
        {
            if (instance == null) return;
            if (!_active.Contains(instance))
            {
                Debug.LogWarning($"[ObjectPool] {typeof(T).Name} 인스턴스가 활성 목록에 없음");
                return;
            }

            _active.Remove(instance);
            instance.OnDespawn();
            instance.gameObject.SetActive(false);
            instance.transform.SetParent(_container);
            _pooled.Push(instance);
        }

        /// <summary>
        /// 모든 활성 객체 반환
        /// </summary>
        public void DespawnAll()
        {
            // ToArray로 복사 후 순회 (컬렉션 수정 방지)
            foreach (var instance in new List<T>(_active))
            {
                Despawn(instance);
            }
        }

        /// <summary>
        /// 풀 정리 (모든 객체 Destroy)
        /// </summary>
        public void Clear()
        {
            DespawnAll();

            while (_pooled.Count > 0)
            {
                var instance = _pooled.Pop();
                if (instance != null)
                {
                    Object.Destroy(instance.gameObject);
                }
            }

            _active.Clear();
        }

        private T CreateInstance()
        {
            var instance = Object.Instantiate(_prefab, _container);
            instance.name = $"{_prefab.name}_{TotalCount}";
            return instance;
        }
    }
}
