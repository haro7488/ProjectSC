using System;

namespace Sc.Data
{
    /// <summary>
    /// 캐릭터 돌파 요청
    /// </summary>
    [Serializable]
    public class CharacterAscensionRequest : IRequest<CharacterAscensionResponse>
    {
        /// <summary>
        /// 캐릭터 인스턴스 ID
        /// </summary>
        public string CharacterInstanceId;

        /// <summary>
        /// 요청 타임스탬프
        /// </summary>
        public long Timestamp { get; private set; }

        public CharacterAscensionRequest()
        {
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        /// <summary>
        /// 돌파 요청 생성
        /// </summary>
        public static CharacterAscensionRequest Create(string characterInstanceId)
        {
            return new CharacterAscensionRequest
            {
                CharacterInstanceId = characterInstanceId
            };
        }
    }
}
