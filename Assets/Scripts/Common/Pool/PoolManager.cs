using System;
using System.Collections.Generic;
using Sc.Foundation;
using UnityEngine;

namespace Sc.Common
{
    /// <summary>
    /// 오브젝트 풀 중앙 관리자
    /// </summary>
    public class PoolManager : Singleton<PoolManager>
    {
        private readonly Dictionary<Type, object> _pools = new();
        private Transform _poolContainer;

        protected override void OnSingletonAwake()
        {
            // 풀 컨테이너 생성
            _poolContainer = new GameObject("[PoolContainer]").transform;
            _poolContainer.SetParent(transform);
        }

        protected override void OnSingletonDestroy()
        {
            ClearAllPools();
        }

        /// <summary>
        /// 풀 생성
        /// </summary>
        /// <typeparam name="T">풀링할 타입</typeparam>
        /// <param name="prefab">프리팹</param>
        /// <param name="initialSize">초기 생성 수</param>
        /// <param name="maxSize">최대 크기 (0 = 무제한)</param>
        public void CreatePool<T>(T prefab, int initialSize = 10, int maxSize = 50) where T : Component, IPoolable
        {
            var type = typeof(T);

            if (_pools.ContainsKey(type))
            {
                Debug.LogWarning($"[PoolManager] {type.Name} 풀이 이미 존재함");
                return;
            }

            // 타입별 컨테이너 생성
            var container = new GameObject($"[Pool_{type.Name}]").transform;
            container.SetParent(_poolContainer);

            var pool = new ObjectPool<T>(prefab, container, initialSize, maxSize);
            _pools[type] = pool;
        }

        /// <summary>
        /// 풀 존재 여부 확인
        /// </summary>
        public bool HasPool<T>() where T : Component, IPoolable
        {
            return _pools.ContainsKey(typeof(T));
        }

        /// <summary>
        /// 풀에서 객체 스폰
        /// </summary>
        public T Spawn<T>() where T : Component, IPoolable
        {
            var pool = GetPool<T>();
            return pool?.Spawn();
        }

        /// <summary>
        /// 위치 지정 스폰
        /// </summary>
        public T Spawn<T>(Vector3 position) where T : Component, IPoolable
        {
            var pool = GetPool<T>();
            return pool?.Spawn(position);
        }

        /// <summary>
        /// 위치 및 회전 지정 스폰
        /// </summary>
        public T Spawn<T>(Vector3 position, Quaternion rotation) where T : Component, IPoolable
        {
            var pool = GetPool<T>();
            return pool?.Spawn(position, rotation);
        }

        /// <summary>
        /// 풀에 반환
        /// </summary>
        public void Despawn<T>(T instance) where T : Component, IPoolable
        {
            var pool = GetPool<T>();
            pool?.Despawn(instance);
        }

        /// <summary>
        /// 타입별 전체 반환
        /// </summary>
        public void DespawnAll<T>() where T : Component, IPoolable
        {
            var pool = GetPool<T>();
            pool?.DespawnAll();
        }

        /// <summary>
        /// 특정 풀 정리
        /// </summary>
        public void ClearPool<T>() where T : Component, IPoolable
        {
            var type = typeof(T);

            if (_pools.TryGetValue(type, out var poolObj))
            {
                var pool = (ObjectPool<T>)poolObj;
                pool.Clear();
                _pools.Remove(type);

                // 컨테이너도 제거
                var container = _poolContainer.Find($"[Pool_{type.Name}]");
                if (container != null)
                {
                    Destroy(container.gameObject);
                }
            }
        }

        /// <summary>
        /// 모든 풀 정리
        /// </summary>
        public void ClearAllPools()
        {
            foreach (var poolObj in _pools.Values)
            {
                // 리플렉션 없이 Clear 호출하기 위해 dynamic 사용 대신 인터페이스 활용
                if (poolObj is IClearable clearable)
                {
                    clearable.Clear();
                }
            }

            _pools.Clear();

            // 모든 컨테이너 제거
            if (_poolContainer != null)
            {
                foreach (Transform child in _poolContainer)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        /// <summary>
        /// 풀 통계 정보
        /// </summary>
        public (int pooled, int active) GetStats<T>() where T : Component, IPoolable
        {
            var pool = GetPool<T>();
            return pool != null ? (pool.PooledCount, pool.ActiveCount) : (0, 0);
        }

        private ObjectPool<T> GetPool<T>() where T : Component, IPoolable
        {
            var type = typeof(T);

            if (!_pools.TryGetValue(type, out var poolObj))
            {
                Debug.LogWarning($"[PoolManager] {type.Name} 풀이 존재하지 않음. CreatePool을 먼저 호출하세요.");
                return null;
            }

            return (ObjectPool<T>)poolObj;
        }
    }

}
