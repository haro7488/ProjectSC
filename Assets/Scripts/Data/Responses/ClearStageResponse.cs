using System;
using System.Collections.Generic;

namespace Sc.Data
{
    /// <summary>
    /// 스테이지 클리어 응답
    /// </summary>
    [Serializable]
    public class ClearStageResponse : IGameActionResponse
    {
        /// <summary>
        /// 요청 성공 여부
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 에러 코드
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// 에러 메시지
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 서버 타임스탬프
        /// </summary>
        public long ServerTime { get; set; }

        /// <summary>
        /// 유저 데이터 변경분
        /// </summary>
        public UserDataDelta Delta { get; set; }

        /// <summary>
        /// 갱신된 클리어 정보
        /// </summary>
        public StageClearInfo ClearInfo;

        /// <summary>
        /// 이번에 새로 달성한 별 [star1, star2, star3]
        /// </summary>
        public bool[] NewStarsAchieved;

        /// <summary>
        /// 첫 클리어 여부
        /// </summary>
        public bool IsFirstClear;

        /// <summary>
        /// 획득한 보상 목록
        /// </summary>
        public List<RewardInfo> TotalRewards;

        /// <summary>
        /// 성공 응답 생성
        /// </summary>
        public static ClearStageResponse Success(
            StageClearInfo clearInfo,
            bool[] newStarsAchieved,
            bool isFirstClear,
            List<RewardInfo> totalRewards,
            UserDataDelta delta)
        {
            return new ClearStageResponse
            {
                IsSuccess = true,
                ErrorCode = 0,
                ErrorMessage = null,
                ServerTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Delta = delta,
                ClearInfo = clearInfo,
                NewStarsAchieved = newStarsAchieved ?? new bool[3],
                IsFirstClear = isFirstClear,
                TotalRewards = totalRewards ?? new List<RewardInfo>()
            };
        }

        /// <summary>
        /// 실패 응답 생성
        /// </summary>
        public static ClearStageResponse Fail(int errorCode, string errorMessage)
        {
            return new ClearStageResponse
            {
                IsSuccess = false,
                ErrorCode = errorCode,
                ErrorMessage = errorMessage,
                ServerTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Delta = UserDataDelta.Empty(),
                TotalRewards = new List<RewardInfo>()
            };
        }
    }
}