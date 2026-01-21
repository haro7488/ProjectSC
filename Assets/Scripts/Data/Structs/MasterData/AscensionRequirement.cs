using System;
using System.Collections.Generic;

namespace Sc.Data
{
    /// <summary>
    /// 돌파 요구사항
    /// </summary>
    [Serializable]
    public struct AscensionRequirement
    {
        /// <summary>
        /// 돌파 단계 (0→1, 1→2, ...)
        /// </summary>
        public int AscensionLevel;

        /// <summary>
        /// 필요 캐릭터 레벨
        /// </summary>
        public int RequiredCharacterLevel;

        /// <summary>
        /// 필요 재료 (ItemId, Amount)
        /// </summary>
        public List<RewardInfo> Materials;

        /// <summary>
        /// 골드 비용
        /// </summary>
        public int GoldCost;

        /// <summary>
        /// 돌파 시 스탯 보너스
        /// </summary>
        public CharacterStats StatBonus;

        /// <summary>
        /// 레벨 상한 증가량
        /// </summary>
        public int LevelCapIncrease;
    }
}
