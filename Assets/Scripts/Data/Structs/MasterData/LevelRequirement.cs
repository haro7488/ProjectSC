using System;

namespace Sc.Data
{
    /// <summary>
    /// 레벨업 요구사항
    /// </summary>
    [Serializable]
    public struct LevelRequirement
    {
        /// <summary>
        /// 목표 레벨
        /// </summary>
        public int Level;

        /// <summary>
        /// 필요 누적 경험치
        /// </summary>
        public long RequiredExp;

        /// <summary>
        /// 레벨업 비용 (골드)
        /// </summary>
        public int GoldCost;

        public LevelRequirement(int level, long requiredExp, int goldCost)
        {
            Level = level;
            RequiredExp = requiredExp;
            GoldCost = goldCost;
        }
    }
}
