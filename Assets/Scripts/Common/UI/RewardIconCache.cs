using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sc.Core;
using Sc.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Sc.Common.UI
{
    /// <summary>
    /// 보상 아이콘 프리로드 캐시.
    /// 팝업 열기 전 아이콘을 미리 로드하여 튀는 현상 방지.
    /// </summary>
    public class RewardIconCache
    {
        private readonly Dictionary<string, Sprite> _cache = new();
        private readonly List<AsyncOperationHandle<Sprite>> _handles = new();
        private Sprite _fallbackIcon;

        /// <summary>
        /// 폴백 아이콘 설정 (로드 실패 시 사용)
        /// </summary>
        public void SetFallbackIcon(Sprite fallback)
        {
            _fallbackIcon = fallback;
        }

        /// <summary>
        /// 보상 목록의 아이콘 일괄 프리로드
        /// </summary>
        public async UniTask PreloadAsync(RewardInfo[] rewards)
        {
            if (rewards == null || rewards.Length == 0) return;

            var paths = new HashSet<string>();
            foreach (var reward in rewards)
            {
                var path = RewardHelper.GetIconPath(reward);
                if (!string.IsNullOrEmpty(path) && !_cache.ContainsKey(path))
                {
                    paths.Add(path);
                }
            }

            if (paths.Count == 0) return;

            var tasks = new List<UniTask>();
            foreach (var path in paths)
            {
                tasks.Add(LoadIconAsync(path));
            }

            await UniTask.WhenAll(tasks);
        }

        private async UniTask LoadIconAsync(string path)
        {
            try
            {
                var handle = Addressables.LoadAssetAsync<Sprite>(path);
                _handles.Add(handle);

                var sprite = await handle.ToUniTask();
                if (sprite != null)
                {
                    _cache[path] = sprite;
                }
                else
                {
                    Debug.LogWarning($"[RewardIconCache] Icon not found: {path}");
                    _cache[path] = _fallbackIcon;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[RewardIconCache] Failed to load icon: {path}, {e.Message}");
                _cache[path] = _fallbackIcon;
            }
        }

        /// <summary>
        /// 캐시된 아이콘 조회 (프리로드 후 사용)
        /// </summary>
        public Sprite GetIcon(RewardInfo reward)
        {
            var path = RewardHelper.GetIconPath(reward);
            return GetIcon(path);
        }

        /// <summary>
        /// 경로로 캐시된 아이콘 조회
        /// </summary>
        public Sprite GetIcon(string path)
        {
            if (_cache.TryGetValue(path, out var sprite))
            {
                return sprite;
            }
            return _fallbackIcon;
        }

        /// <summary>
        /// 아이콘이 캐시되어 있는지 확인
        /// </summary>
        public bool HasIcon(string path)
        {
            return _cache.ContainsKey(path);
        }

        /// <summary>
        /// 캐시 및 Addressables 핸들 해제
        /// </summary>
        public void Release()
        {
            foreach (var handle in _handles)
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }
            _handles.Clear();
            _cache.Clear();
        }
    }
}
