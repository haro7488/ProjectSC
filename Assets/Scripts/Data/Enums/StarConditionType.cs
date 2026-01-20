namespace Sc.Data
{
    /// <summary>
    /// 별점 조건 타입
    /// </summary>
    public enum StarConditionType
    {
        /// <summary>
        /// 클리어
        /// </summary>
        Clear = 0,

        /// <summary>
        /// N턴 이내 클리어
        /// </summary>
        TurnLimit = 1,

        /// <summary>
        /// 사망자 없이 클리어
        /// </summary>
        NoCharacterDeath = 2,

        /// <summary>
        /// 아군 전원 HP 100%
        /// </summary>
        FullHP = 3,

        /// <summary>
        /// 유리 속성으로 클리어
        /// </summary>
        ElementAdvantage = 4,
    }
}