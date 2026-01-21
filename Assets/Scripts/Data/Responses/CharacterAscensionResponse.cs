using System;

namespace Sc.Data
{
    /// <summary>
    /// 캐릭터 돌파 응답
    /// </summary>
    [Serializable]
    public class CharacterAscensionResponse : IGameActionResponse
    {
        public bool IsSuccess { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public long ServerTime { get; set; }
        public UserDataDelta Delta { get; set; }

        /// <summary>
        /// 이전 돌파 단계
        /// </summary>
        public int PreviousAscension;

        /// <summary>
        /// 새 돌파 단계
        /// </summary>
        public int NewAscension;

        /// <summary>
        /// 이전 레벨 상한
        /// </summary>
        public int PreviousLevelCap;

        /// <summary>
        /// 새 레벨 상한
        /// </summary>
        public int NewLevelCap;

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
        public static CharacterAscensionResponse Success(
            int prevAscension, int newAscension,
            int prevLevelCap, int newLevelCap,
            CharacterStats prevStats, CharacterStats newStats,
            int prevPower, int newPower,
            UserDataDelta delta)
        {
            return new CharacterAscensionResponse
            {
                IsSuccess = true,
                ErrorCode = 0,
                ErrorMessage = null,
                ServerTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Delta = delta,
                PreviousAscension = prevAscension,
                NewAscension = newAscension,
                PreviousLevelCap = prevLevelCap,
                NewLevelCap = newLevelCap,
                PreviousStats = prevStats,
                NewStats = newStats,
                PreviousPower = prevPower,
                NewPower = newPower
            };
        }

        /// <summary>
        /// 실패 응답 생성
        /// </summary>
        public static CharacterAscensionResponse Fail(int errorCode, string errorMessage)
        {
            return new CharacterAscensionResponse
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
