using System;

namespace Sc.Data
{
    /// <summary>
    /// 캐릭터 레벨업 응답
    /// </summary>
    [Serializable]
    public class CharacterLevelUpResponse : IGameActionResponse
    {
        public bool IsSuccess { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public long ServerTime { get; set; }
        public UserDataDelta Delta { get; set; }

        /// <summary>
        /// 이전 레벨
        /// </summary>
        public int PreviousLevel;

        /// <summary>
        /// 새 레벨
        /// </summary>
        public int NewLevel;

        /// <summary>
        /// 이전 경험치
        /// </summary>
        public long PreviousExp;

        /// <summary>
        /// 새 경험치
        /// </summary>
        public long NewExp;

        /// <summary>
        /// 이전 스탯
        /// </summary>
        public CharacterStats PreviousStats;

        /// <summary>
        /// 새 스탯
        /// </summary>
        public CharacterStats NewStats;

        /// <summary>
        /// 이전 전투력
        /// </summary>
        public int PreviousPower;

        /// <summary>
        /// 새 전투력
        /// </summary>
        public int NewPower;

        /// <summary>
        /// 성공 응답 생성
        /// </summary>
        public static CharacterLevelUpResponse Success(
            int prevLevel, int newLevel,
            long prevExp, long newExp,
            CharacterStats prevStats, CharacterStats newStats,
            int prevPower, int newPower,
            UserDataDelta delta)
        {
            return new CharacterLevelUpResponse
            {
                IsSuccess = true,
                ErrorCode = 0,
                ErrorMessage = null,
                ServerTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Delta = delta,
                PreviousLevel = prevLevel,
                NewLevel = newLevel,
                PreviousExp = prevExp,
                NewExp = newExp,
                PreviousStats = prevStats,
                NewStats = newStats,
                PreviousPower = prevPower,
                NewPower = newPower
            };
        }

        /// <summary>
        /// 실패 응답 생성
        /// </summary>
        public static CharacterLevelUpResponse Fail(int errorCode, string errorMessage)
        {
            return new CharacterLevelUpResponse
            {
                IsSuccess = false,
                ErrorCode = errorCode,
                ErrorMessage = errorMessage,
                ServerTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Delta = UserDataDelta.Empty()
            };
        }
    }
}
