using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sc.Data
{
    /// <summary>
    /// 캐릭터 돌파 요구사항 데이터베이스
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterAscensionDatabase", menuName = "SC/Database/CharacterAscension")]
    public class CharacterAscensionDatabase : ScriptableObject
    {
        [Serializable]
        public class RarityAscensionData
        {
            public Rarity Rarity;
            public List<AscensionRequirement> Requirements = new();
        }

        [SerializeField] private List<RarityAscensionData> _data = new();

        private Dictionary<Rarity, RarityAscensionData> _lookup;

        /// <summary>
        /// 특정 레어도의 돌파 요구사항 조회
        /// </summary>
        public AscensionRequirement? GetRequirement(Rarity rarity, int ascensionLevel)
        {
            EnsureLookup();
            if (!_lookup.TryGetValue(rarity, out var data)) return null;
            return data.Requirements.FirstOrDefault(r => r.AscensionLevel == ascensionLevel);
        }

        /// <summary>
        /// 특정 레어도의 최대 돌파 단계 조회
        /// </summary>
        public int GetMaxAscension(Rarity rarity)
        {
            EnsureLookup();
            if (!_lookup.TryGetValue(rarity, out var data)) return 0;
            return data.Requirements.Count;
        }

        /// <summary>
        /// 돌파 단계에 따른 레벨 상한 계산
        /// </summary>
        public int GetLevelCap(Rarity rarity, int ascension, int baseLevelCap)
        {
            EnsureLookup();
            if (!_lookup.TryGetValue(rarity, out var data)) return baseLevelCap;

            int levelCap = baseLevelCap;
            for (int i = 0; i < ascension && i < data.Requirements.Count; i++)
            {
                levelCap += data.Requirements[i].LevelCapIncrease;
            }
            return levelCap;
        }

        /// <summary>
        /// 돌파 단계까지의 누적 스탯 보너스
        /// </summary>
        public CharacterStats GetTotalStatBonus(Rarity rarity, int ascension)
        {
            EnsureLookup();
            if (!_lookup.TryGetValue(rarity, out var data)) return default;

            var total = new CharacterStats();
            for (int i = 0; i < ascension && i < data.Requirements.Count; i++)
            {
                total = total + data.Requirements[i].StatBonus;
            }
            return total;
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
        public void SetData(List<RarityAscensionData> data)
        {
            _data = data;
            _lookup = null;
        }
#endif
    }
}
