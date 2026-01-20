using System.Collections.Generic;
using Sc.Data;

namespace Sc.Event.OutGame
{
    #region Stage Entry

    /// <summary>
    /// 스테이지 입장 완료 이벤트
    /// </summary>
    public readonly struct StageEnteredEvent
    {
        /// <summary>
        /// 입장한 스테이지 ID
        /// </summary>
        public string StageId { get; init; }

        /// <summary>
        /// 전투 세션 ID
        /// </summary>
        public string BattleSessionId { get; init; }

        /// <summary>
        /// 갱신된 입장 기록
        /// </summary>
        public StageEntryRecord EntryRecord { get; init; }

        /// <summary>
        /// 유저 데이터 변경분
        /// </summary>
        public UserDataDelta Delta { get; init; }
    }

    /// <summary>
    /// 스테이지 입장 실패 이벤트
    /// </summary>
    public readonly struct StageEntryFailedEvent
    {
        /// <summary>
        /// 입장 시도한 스테이지 ID
        /// </summary>
        public string StageId { get; init; }

        /// <summary>
        /// 에러 코드
        /// </summary>
        public int ErrorCode { get; init; }

        /// <summary>
        /// 에러 메시지
        /// </summary>
        public string ErrorMessage { get; init; }
    }

    #endregion

    #region Stage Clear

    /// <summary>
    /// 스테이지 클리어 완료 이벤트
    /// </summary>
    public readonly struct StageClearedEvent
    {
        /// <summary>
        /// 클리어한 스테이지 ID
        /// </summary>
        public string StageId { get; init; }

        /// <summary>
        /// 승리 여부
        /// </summary>
        public bool IsVictory { get; init; }

        /// <summary>
        /// 첫 클리어 여부
        /// </summary>
        public bool IsFirstClear { get; init; }

        /// <summary>
        /// 이번에 새로 달성한 별 [star1, star2, star3]
        /// </summary>
        public bool[] NewStarsAchieved { get; init; }

        /// <summary>
        /// 획득한 보상 목록
        /// </summary>
        public List<RewardInfo> Rewards { get; init; }

        /// <summary>
        /// 갱신된 클리어 정보
        /// </summary>
        public StageClearInfo ClearInfo { get; init; }

        /// <summary>
        /// 유저 데이터 변경분
        /// </summary>
        public UserDataDelta Delta { get; init; }
    }

    /// <summary>
    /// 스테이지 클리어 실패 이벤트
    /// </summary>
    public readonly struct StageClearFailedEvent
    {
        /// <summary>
        /// 전투 세션 ID
        /// </summary>
        public string BattleSessionId { get; init; }

        /// <summary>
        /// 에러 코드
        /// </summary>
        public int ErrorCode { get; init; }

        /// <summary>
        /// 에러 메시지
        /// </summary>
        public string ErrorMessage { get; init; }
    }

    #endregion

    #region Battle Ready

    /// <summary>
    /// 전투 준비 완료 이벤트 (Battle 시스템으로 전달)
    /// </summary>
    public readonly struct BattleReadyEvent
    {
        /// <summary>
        /// 전투 세션 ID
        /// </summary>
        public string BattleSessionId { get; init; }

        /// <summary>
        /// 스테이지 ID
        /// </summary>
        public string StageId { get; init; }

        /// <summary>
        /// 파티 캐릭터 Instance ID 목록
        /// </summary>
        public List<string> PartyCharacterIds { get; init; }
    }

    #endregion
}