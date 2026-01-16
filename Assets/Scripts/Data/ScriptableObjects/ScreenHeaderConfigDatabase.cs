using System.Collections.Generic;
using UnityEngine;

namespace Sc.Data
{
    /// <summary>
    /// Screen Header 설정 데이터베이스 (마스터 데이터 컬렉션)
    /// </summary>
    [CreateAssetMenu(fileName = "ScreenHeaderConfigDatabase", menuName = "SC/Database/ScreenHeaderConfig")]
    public class ScreenHeaderConfigDatabase : ScriptableObject
    {
        [SerializeField] private List<ScreenHeaderConfigData> _configs = new();

        private Dictionary<string, ScreenHeaderConfigData> _lookup;

        /// <summary>
        /// 전체 설정 목록
        /// </summary>
        public IReadOnlyList<ScreenHeaderConfigData> Configs => _configs;

        /// <summary>
        /// 설정 수
        /// </summary>
        public int Count => _configs.Count;

        /// <summary>
        /// ID로 설정 조회
        /// </summary>
        public ScreenHeaderConfigData GetById(string id)
        {
            EnsureLookup();
            return _lookup.TryGetValue(id, out var data) ? data : null;
        }

        /// <summary>
        /// ID 존재 여부 확인
        /// </summary>
        public bool Contains(string id)
        {
            EnsureLookup();
            return _lookup.ContainsKey(id);
        }

        private void EnsureLookup()
        {
            if (_lookup != null) return;

            _lookup = new Dictionary<string, ScreenHeaderConfigData>(_configs.Count);
            foreach (var config in _configs)
            {
                if (config != null && !string.IsNullOrEmpty(config.Id))
                {
                    _lookup[config.Id] = config;
                }
            }
        }

        private void OnEnable()
        {
            _lookup = null; // 재생성 강제
        }

#if UNITY_EDITOR
        /// <summary>
        /// Editor 전용: 설정 추가
        /// </summary>
        public void Add(ScreenHeaderConfigData data)
        {
            if (data != null && !_configs.Contains(data))
            {
                _configs.Add(data);
                _lookup = null;
            }
        }

        /// <summary>
        /// Editor 전용: 전체 클리어
        /// </summary>
        public void Clear()
        {
            _configs.Clear();
            _lookup = null;
        }
#endif
    }
}
