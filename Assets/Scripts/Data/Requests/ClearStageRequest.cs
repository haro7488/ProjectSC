using System;

namespace Sc.Data
{
    /// <summary>
    /// 스테이지 클리어 요청
    /// </summary>
    [Serializable]
    public class ClearStageRequest : IRequest<ClearStageResponse>
    {
        /// <summary>
        /// 전투 세션 ID
        /// </summary>
        public string BattleSessionId;

        /// <summary>
        /// 승리 여부
        /// </summary>
        public bool IsVictory;

        /// <summary>
        /// 사용한 턴 수
        /// </summary>
        public int TurnCount;

        /// <summary>
        /// 캐릭터 사망 없이 클리어 여부
        /// </summary>
        public bool NoCharacterDeath;

        /// <summary>
        /// 전원 풀피로 클리어 여부
        /// </summary>
        public bool AllFullHP;

        /// <summary>
        /// 요청 타임스탬프
        /// </summary>
        public long Timestamp { get; private set; }

        public ClearStageRequest()
        {
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        /// <summary>
        /// 클리어 요청 생성
        /// </summary>
        public static ClearStageRequest Create(
            string battleSessionId,
            bool isVictory,
            int turnCount,
            bool noCharacterDeath,
            bool allFullHP)
        {
            return new ClearStageRequest
            {
                BattleSessionId = battleSessionId,
                IsVictory = isVictory,
                TurnCount = turnCount,
                NoCharacterDeath = noCharacterDeath,
                AllFullHP = allFullHP
            };
        }
    }
}