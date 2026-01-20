using System;

namespace Sc.Data
{
    /// <summary>
    /// 스테이지 별점 조건
    /// </summary>
    [Serializable]
    public struct StarCondition
    {
        /// <summary>
        /// 조건 타입
        /// </summary>
        public StarConditionType ConditionType;

        /// <summary>
        /// 조건 값 (TurnLimit일 때 턴 수, 기타는 0)
        /// </summary>
        public int Value;

        /// <summary>
        /// 기본 클리어 조건 생성
        /// </summary>
        public static StarCondition Clear()
        {
            return new StarCondition { ConditionType = StarConditionType.Clear, Value = 0 };
        }

        /// <summary>
        /// 턴 제한 조건 생성
        /// </summary>
        public static StarCondition TurnLimit(int turns)
        {
            return new StarCondition { ConditionType = StarConditionType.TurnLimit, Value = turns };
        }

        /// <summary>
        /// 사망자 없이 클리어 조건 생성
        /// </summary>
        public static StarCondition NoCharacterDeath()
        {
            return new StarCondition { ConditionType = StarConditionType.NoCharacterDeath, Value = 0 };
        }

        /// <summary>
        /// 전원 풀피 조건 생성
        /// </summary>
        public static StarCondition FullHP()
        {
            return new StarCondition { ConditionType = StarConditionType.FullHP, Value = 0 };
        }

        /// <summary>
        /// 유리 속성 조건 생성
        /// </summary>
        public static StarCondition ElementAdvantage()
        {
            return new StarCondition { ConditionType = StarConditionType.ElementAdvantage, Value = 0 };
        }

        public override string ToString()
        {
            return ConditionType == StarConditionType.TurnLimit
                ? $"{ConditionType}({Value})"
                : ConditionType.ToString();
        }
    }
}