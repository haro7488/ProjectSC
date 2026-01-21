using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sc.Data
{
    /// <summary>
    /// 캐릭터 레벨 요구사항 데이터베이스
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterLevelDatabase", menuName = "SC/Database/CharacterLevel")]
    public class CharacterLevelDatabase : ScriptableObject
    {
        [Serializable]
        public class RarityLevelData
        {
            public Rarity Rarity;
            public List<LevelRequirement> Requirements = new();
        }

        [SerializeField] private List<RarityLevelData> _data = new();
        [SerializeField] private int _baseLevelCap = 20;

        private Dictionary<Rarity, RarityLevelData> _lookup;

        /// <summary>
        /// 기본 레벨 상한
        /// </summary>
        public int BaseLevelCap => _baseLevelCap;

        /// <summary>
        /// 특정 레어도의 레벨 요구사항 조회
        /// </summary>
        public LevelRequirement? GetRequirement(Rarity rarity, int level)
        {
            EnsureLookup();
            if (!_lookup.TryGetValue(rarity, out var data)) return null;
            return data.Requirements.FirstOrDefault(r => r.Level == level);
        }

        /// <summary>
        /// 특정 레어도의 최대 레벨 조회
        /// </summary>
        public int GetMaxLevel(Rarity rarity)
        {
            EnsureLookup();
            if (!_lookup.TryGetValue(rarity, out var data)) return _baseLevelCap;
            return data.Requirements.Count > 0 
                ? data.Requirements.Max(r => r.Level) 
                : _baseLevelCap;
        }

        /// <summary>
        /// 현재 경험치로 도달 가능한 레벨 계산
        /// </summary>
        public int CalculateLevelFromExp(Rarity rarity, long totalExp, int levelCap)
        {
            EnsureLookup();
            if (!_lookup.TryGetValue(rarity, out var data)) return 1;

            int level = 1;
            foreach (var req in data.Requirements.OrderBy(r => r.Level))
            {
                if (req.Level > levelCap) break;
                if (totalExp >= req.RequiredExp)
                    level = req.Level;
                else
                    break;
            }
            return level;
        }

        /// <summary>
        /// 다음 레벨까지 필요한 경험치
        /// </summary>
        public long GetExpToNextLevel(Rarity rarity, int currentLevel, long currentTotalExp)
        {
            var nextReq = GetRequirement(rarity, currentLevel + 1);
            if (!nextReq.HasValue) return 0;
            return Math.Max(0, nextReq.Value.RequiredExp - currentTotalExp);
        }

        private void EnsureLookup()
        {
            if (_lookup != null) return;
            _lookup = _data.ToDictionary(d => d.Rarity, d => d);
        }

        private void OnEnable()
        {
            _lookup = null;
        }

#if UNITY_EDITOR
        public void SetData(List<RarityLevelData> data, int baseLevelCap)
        {
            _data = data;
            _baseLevelCap = baseLevelCap;
            _lookup = null;
        }
#endif
    }
}
