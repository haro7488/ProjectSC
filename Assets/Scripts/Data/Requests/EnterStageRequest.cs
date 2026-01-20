using System;
using System.Collections.Generic;

namespace Sc.Data
{
    /// <summary>
    /// 스테이지 입장 요청
    /// </summary>
    [Serializable]
    public class EnterStageRequest : IRequest<EnterStageResponse>
    {
        /// <summary>
        /// 스테이지 ID
        /// </summary>
        public string StageId;

        /// <summary>
        /// 파티 캐릭터 Instance ID 목록
        /// </summary>
        public List<string> PartyCharacterIds;

        /// <summary>
        /// 요청 타임스탬프
        /// </summary>
        public long Timestamp { get; private set; }

        public EnterStageRequest()
        {
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        /// <summary>
        /// 입장 요청 생성
        /// </summary>
        public static EnterStageRequest Create(string stageId, List<string> partyCharacterIds)
        {
            return new EnterStageRequest
            {
                StageId = stageId,
                PartyCharacterIds = partyCharacterIds ?? new List<string>()
            };
        }
    }
}