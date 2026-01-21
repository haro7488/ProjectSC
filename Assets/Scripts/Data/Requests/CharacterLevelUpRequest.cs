using System;
using System.Collections.Generic;

namespace Sc.Data
{
    /// <summary>
    /// 캐릭터 레벨업 요청
    /// </summary>
    [Serializable]
    public class CharacterLevelUpRequest : IRequest<CharacterLevelUpResponse>
    {
        /// <summary>
        /// 캐릭터 인스턴스 ID
        /// </summary>
        public string CharacterInstanceId;

        /// <summary>
        /// 사용할 재료 (ItemId → 수량)
        /// </summary>
        public Dictionary<string, int> MaterialUsage;

        /// <summary>
        /// 요청 타임스탬프
        /// </summary>
        public long Timestamp { get; private set; }

        public CharacterLevelUpRequest()
        {
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            MaterialUsage = new Dictionary<string, int>();
        }

        /// <summary>
        /// 레벨업 요청 생성
        /// </summary>
        public static CharacterLevelUpRequest Create(string characterInstanceId, Dictionary<string, int> materials)
        {
            return new CharacterLevelUpRequest
            {
                CharacterInstanceId = characterInstanceId,
                MaterialUsage = materials ?? new Dictionary<string, int>()
            };
        }
    }
}
