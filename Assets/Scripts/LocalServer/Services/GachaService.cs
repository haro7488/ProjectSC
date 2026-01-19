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
        // 기본 확률 (실제로는 GachaPoolData에서 읽어와야 함)
        private const float SSR_RATE = 0.03f;
        private const float SR_RATE = 0.12f;
        private const int PITY_THRESHOLD = 90;

        /// <summary>
        /// 가챠 희귀도 계산 (천장 시스템 포함)
        /// </summary>
        public Rarity CalculateRarity(int pityCount)
        {
            // 천장 도달 시 SSR 확정
            if (pityCount >= PITY_THRESHOLD)
                return Rarity.SSR;

            var random = Random.Range(0f, 1f);

            if (random < SSR_RATE)
                return Rarity.SSR;
            if (random < SSR_RATE + SR_RATE)
                return Rarity.SR;
            return Rarity.R;
        }

        /// <summary>
        /// 희귀도에 따른 랜덤 캐릭터 선택
        /// </summary>
        /// <remarks>
        /// 실제 구현에서는 GachaPoolData의 캐릭터 목록에서 선택해야 함
        /// 현재는 더미 데이터 반환
        /// </remarks>
        public string GetRandomCharacterByRarity(Rarity rarity, string poolId)
        {
            // TODO: GachaPoolData에서 해당 poolId의 캐릭터 목록 조회
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
        public (int costPerPull, int totalCost) CalculateCost(GachaPullType pullType)
        {
            const int costPerPull = 300;
            var pullCount = pullType == GachaPullType.Multi ? 10 : 1;
            var totalCost = pullType == GachaPullType.Multi ? 2700 : costPerPull; // 10연차 할인

            return (costPerPull, totalCost);
        }

        /// <summary>
        /// 뽑기 횟수 반환
        /// </summary>
        public int GetPullCount(GachaPullType pullType)
        {
            return pullType == GachaPullType.Multi ? 10 : 1;
        }
    }
}
