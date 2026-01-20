using System;

namespace Sc.Data
{
    /// <summary>
    /// 스테이지 입장 응답
    /// </summary>
    [Serializable]
    public class EnterStageResponse : IGameActionResponse
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
        /// 전투 세션 ID
        /// </summary>
        public string BattleSessionId;

        /// <summary>
        /// 갱신된 입장 기록
        /// </summary>
        public StageEntryRecord EntryRecord;

        /// <summary>
        /// 성공 응답 생성
        /// </summary>
        public static EnterStageResponse Success(
            string battleSessionId,
            StageEntryRecord entryRecord,
            UserDataDelta delta)
        {
            return new EnterStageResponse
            {
                IsSuccess = true,
                ErrorCode = 0,
                ErrorMessage = null,
                ServerTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Delta = delta,
                BattleSessionId = battleSessionId,
                EntryRecord = entryRecord
            };
        }

        /// <summary>
        /// 실패 응답 생성
        /// </summary>
        public static EnterStageResponse Fail(int errorCode, string errorMessage)
        {
            return new EnterStageResponse
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