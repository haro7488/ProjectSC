using System;
using System.Collections.Generic;

namespace Sc.Data
{
    /// <summary>
    /// 전투 세션 데이터
    /// </summary>
    [Serializable]
    public struct BattleSessionData
    {
        /// <summary>
        /// 세션 ID (GUID)
        /// </summary>
        public string SessionId;

        /// <summary>
        /// 스테이지 ID
        /// </summary>
        public string StageId;

        /// <summary>
        /// 파티 캐릭터 Instance ID 목록
        /// </summary>
        public List<string> PartyCharacterIds;

        /// <summary>
        /// 세션 생성 시간 (Unix Timestamp)
        /// </summary>
        public long CreatedAt;

        /// <summary>
        /// 세션 활성 상태 (false = 클리어/만료됨)
        /// </summary>
        public bool IsActive;

        /// <summary>
        /// 생성자
        /// </summary>
        public BattleSessionData(
            string sessionId,
            string stageId,
            List<string> partyCharacterIds,
            long createdAt,
            bool isActive)
        {
            SessionId = sessionId;
            StageId = stageId;
            PartyCharacterIds = partyCharacterIds;
            CreatedAt = createdAt;
            IsActive = isActive;
        }

        /// <summary>
        /// 새 전투 세션 생성
        /// </summary>
        public static BattleSessionData Create(string stageId, List<string> partyIds, long createdAt)
        {
            return new BattleSessionData(
                Guid.NewGuid().ToString(),
                stageId,
                partyIds,
                createdAt,
                true
            );
        }

        /// <summary>
        /// 세션 만료 여부 확인
        /// </summary>
        /// <param name="currentTime">현재 시간 (Unix Timestamp)</param>
        /// <param name="expirationSeconds">만료 시간 (기본 1시간)</param>
        public bool IsExpired(long currentTime, int expirationSeconds = 3600)
        {
            return currentTime - CreatedAt > expirationSeconds;
        }

        /// <summary>
        /// 세션 비활성화
        /// </summary>
        public BattleSessionData Deactivate()
        {
            return new BattleSessionData(
                SessionId,
                StageId,
                PartyCharacterIds,
                CreatedAt,
                false
            );
        }

        public override string ToString()
        {
            return $"[Session:{SessionId}] Stage:{StageId}, Active:{IsActive}";
        }
    }
}