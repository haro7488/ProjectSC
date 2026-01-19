using System.Collections.Generic;
using UnityEngine;

namespace Sc.Common.UI
{
    /// <summary>
    /// Instantiate/Destroy 기반 아이템 생성기.
    /// 풀링 없이 단순하게 동작하며, 이후 PooledItemSpawner로 교체 가능.
    /// </summary>
    public class SimpleItemSpawner<T> : IItemSpawner<T> where T : Component
    {
        private readonly T _prefab;
        private readonly List<T> _spawnedItems = new();

        public int ActiveCount => _spawnedItems.Count;

        public SimpleItemSpawner(T prefab)
        {
            _prefab = prefab;
        }

        public T Spawn(Transform parent)
        {
            if (_prefab == null)
            {
                Debug.LogError("[SimpleItemSpawner] Prefab is null");
                return null;
            }

            var item = Object.Instantiate(_prefab, parent);
            _spawnedItems.Add(item);
            return item;
        }

        public void Despawn(T item)
        {
            if (item == null) return;

            _spawnedItems.Remove(item);
            Object.Destroy(item.gameObject);
        }

        public void DespawnAll()
        {
            foreach (var item in _spawnedItems)
            {
                if (item != null)
                {
                    Object.Destroy(item.gameObject);
                }
            }
            _spawnedItems.Clear();
        }
    }
}
