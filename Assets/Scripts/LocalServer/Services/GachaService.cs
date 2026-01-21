using System.Collections.Generic;
using Sc.Data;
using UnityEngine;

namespace Sc.LocalServer
{
    /// <summary>
    /// 가챠 서비스 (서버측)
    /// 확률 계산, 캐릭터 선택 등 가챠 핵심 로직
    /// </summary>
    public class GachaService
    {
        // 기본 확률 (풀 데이터가 없을 때 사용)
        private const float DEFAULT_SSR_RATE = 0.03f;
        private const float DEFAULT_SR_RATE = 0.12f;
        private const int DEFAULT_PITY_THRESHOLD = 90;

        private CharacterDatabase _characterDatabase;

        /// <summary>
        /// CharacterDatabase 설정 (외부에서 주입)
        /// </summary>
        public void SetCharacterDatabase(CharacterDatabase database)
        {
            _characterDatabase = database;
        }

        /// <summary>
        /// 가챠 희귀도 계산 (소프트 천장 시스템 포함)
        /// </summary>
        public Rarity CalculateRarity(int pityCount, GachaPoolData poolData = null)
        {
            var ssrRate = poolData?.Rates.SSR ?? DEFAULT_SSR_RATE;
            var srRate = poolData?.Rates.SR ?? DEFAULT_SR_RATE;
            var pityThreshold = poolData?.PityCount ?? DEFAULT_PITY_THRESHOLD;
            var pitySoftStart = poolData?.PitySoftStart ?? 0;
            var pitySoftRateBonus = poolData?.PitySoftRateBonus ?? 0f;

            // 확정 천장 도달 시 SSR 확정
            if (pityThreshold > 0 && pityCount >= pityThreshold)
            {
                return Rarity.SSR;
            }

            // 소프트 천장 확률 증가 계산
            var effectiveSSRRate = ssrRate;
            if (pitySoftStart > 0 && pityCount >= pitySoftStart && pitySoftRateBonus > 0)
            {
                var bonusCount = pityCount - pitySoftStart;
                effectiveSSRRate += bonusCount * pitySoftRateBonus;

                // 최대 100% 제한
                effectiveSSRRate = Mathf.Min(effectiveSSRRate, 1f);
            }

            var random = Random.Range(0f, 1f);

            if (random < effectiveSSRRate)
                return Rarity.SSR;
            if (random < effectiveSSRRate + srRate)
                return Rarity.SR;
            return Rarity.R;
        }

        /// <summary>
        /// 현재 적용되는 SSR 확률 계산 (UI 표시용)
        /// </summary>
        public float GetEffectiveSSRRate(int pityCount, GachaPoolData poolData)
        {
            if (poolData == null) return DEFAULT_SSR_RATE;

            var ssrRate = poolData.Rates.SSR;
            var pityThreshold = poolData.PityCount;
            var pitySoftStart = poolData.PitySoftStart;
            var pitySoftRateBonus = poolData.PitySoftRateBonus;

            // 확정 천장
            if (pityThreshold > 0 && pityCount >= pityThreshold)
            {
                return 1f;
            }

            // 소프트 천장 적용
            if (pitySoftStart > 0 && pityCount >= pitySoftStart && pitySoftRateBonus > 0)
            {
                var bonusCount = pityCount - pitySoftStart;
                var effectiveRate = ssrRate + bonusCount * pitySoftRateBonus;
                return Mathf.Min(effectiveRate, 1f);
            }

            return ssrRate;
        }

        /// <summary>
        /// 희귀도에 따른 랜덤 캐릭터 선택
        /// </summary>
        public string GetRandomCharacterByRarity(Rarity rarity, string poolId, GachaPoolData poolData = null)
        {
            // 풀 데이터에서 캐릭터 목록 조회
            if (poolData != null && poolData.CharacterIds != null && poolData.CharacterIds.Length > 0)
            {
                // 해당 희귀도의 캐릭터 필터링
                var candidates = new List<string>();

                if (_characterDatabase != null)
                {
                    foreach (var charId in poolData.CharacterIds)
                    {
                        var charData = _characterDatabase.GetById(charId);
                        if (charData != null && charData.Rarity == rarity)
                        {
                            candidates.Add(charId);
                        }
                    }
                }

                // 픽업 캐릭터 우선 처리
                if (rarity == Rarity.SSR &&
                    !string.IsNullOrEmpty(poolData.RateUpCharacterId) &&
                    poolData.RateUpBonus > 0)
                {
                    // 픽업 확률 적용
                    if (Random.Range(0f, 1f) < poolData.RateUpBonus)
                    {
                        return poolData.RateUpCharacterId;
                    }
                }

                // 일반 랜덤 선택
                if (candidates.Count > 0)
                {
                    return candidates[Random.Range(0, candidates.Count)];
                }
            }

            // 기본 더미 데이터 반환
            return rarity switch
            {
                Rarity.SSR => Random.Range(0, 2) == 0 ? "char_001" : "char_002",
                Rarity.SR => Random.Range(0, 2) == 0 ? "char_003" : "char_004",
                _ => Random.Range(0, 2) == 0 ? "char_005" : "char_006"
            };
        }

        /// <summary>
        /// 가챠 비용 계산
        /// </summary>
        public (int costPerPull, int totalCost) CalculateCost(GachaPullType pullType, GachaPoolData poolData = null)
        {
            var costSingle = poolData?.CostAmount ?? 300;
            var cost10 = poolData?.CostAmount10 ?? 2700;

            var totalCost = pullType == GachaPullType.Multi ? cost10 : costSingle;
            return (costSingle, totalCost);
        }

        /// <summary>
        /// 뽑기 횟수 반환
        /// </summary>
        public int GetPullCount(GachaPullType pullType)
        {
            return pullType == GachaPullType.Multi ? 10 : 1;
        }

        /// <summary>
        /// 소프트 천장 도달 여부 확인
        /// </summary>
        public bool IsInSoftPityRange(int pityCount, GachaPoolData poolData)
        {
            if (poolData == null) return false;
            return poolData.PitySoftStart > 0 && pityCount >= poolData.PitySoftStart;
        }
    }
}
