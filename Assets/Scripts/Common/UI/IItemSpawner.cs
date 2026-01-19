using UnityEngine;

namespace Sc.Common.UI
{
    /// <summary>
    /// 동적 UI 아이템 생성 추상화 인터페이스.
    /// 1차: SimpleItemSpawner (Instantiate/Destroy)
    /// 2차: PooledItemSpawner (오브젝트 풀링)
    /// </summary>
    public interface IItemSpawner<T> where T : Component
    {
        /// <summary>
        /// 아이템 생성
        /// </summary>
        T Spawn(Transform parent);

        /// <summary>
        /// 아이템 해제
        /// </summary>
        void Despawn(T item);

        /// <summary>
        /// 모든 생성된 아이템 일괄 해제
        /// </summary>
        void DespawnAll();

        /// <summary>
        /// 현재 생성된 아이템 수
        /// </summary>
        int ActiveCount { get; }
    }
}
