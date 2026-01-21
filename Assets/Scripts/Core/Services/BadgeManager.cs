using System;
using System.Collections.Generic;
using Sc.Foundation;

namespace Sc.Core
{
    /// <summary>
    /// 배지 집계/캐시 관리자
    /// 계산 로직은 각 Contents의 IBadgeProvider에 위임
    /// </summary>
    public class BadgeManager : Singleton<BadgeManager>
    {
        public event Action<BadgeType, int> OnBadgeChanged;

        private readonly Dictionary<BadgeType, int> _counts = new();
        private readonly List<IBadgeProvider> _providers = new();

        /// <summary>
        /// Provider 등록 (각 Contents 초기화 시 호출)
        /// </summary>
        public void Register(IBadgeProvider provider)
        {
            if (provider != null && !_providers.Contains(provider))
            {
                _providers.Add(provider);
            }
        }

        /// <summary>
        /// Provider 해제
        /// </summary>
        public void Unregister(IBadgeProvider provider)
        {
            _providers.Remove(provider);
        }

        /// <summary>
        /// 전체 배지 갱신 (로비 진입 시 호출)
        /// </summary>
        public void RefreshAll()
        {
            foreach (var provider in _providers)
            {
                int count = provider.CalculateBadgeCount();
                SetBadge(provider.Type, count);
            }
        }

        /// <summary>
        /// 특정 타입 배지만 갱신
        /// </summary>
        public void Refresh(BadgeType type)
        {
            var provider = _providers.Find(p => p.Type == type);
            if (provider != null)
            {
                int count = provider.CalculateBadgeCount();
                SetBadge(type, count);
            }
        }

        /// <summary>
        /// 배지 직접 설정 (Provider 없이 수동 설정 시)
        /// </summary>
        public void SetBadge(BadgeType type, int count)
        {
            int oldCount = _counts.GetValueOrDefault(type, 0);
            _counts[type] = count;

            if (oldCount != count)
            {
                OnBadgeChanged?.Invoke(type, count);
            }
        }

        /// <summary>
        /// 배지 카운트 조회
        /// </summary>
        public int GetBadge(BadgeType type)
        {
            return _counts.GetValueOrDefault(type, 0);
        }

        /// <summary>
        /// 배지 존재 여부
        /// </summary>
        public bool HasBadge(BadgeType type)
        {
            return GetBadge(type) > 0;
        }

        /// <summary>
        /// 모든 Provider 해제 및 카운트 초기화
        /// </summary>
        public void Clear()
        {
            _providers.Clear();
            _counts.Clear();
        }
    }
}
