using Sc.Data;

namespace Sc.Event.OutGame
{
    #region LevelUp

    /// <summary>
    /// 캐릭터 레벨업 완료 이벤트
    /// </summary>
    public readonly struct CharacterLevelUpEvent
    {
        /// <summary>
        /// 캐릭터 인스턴스 ID
        /// </summary>
        public string CharacterInstanceId { get; init; }

        /// <summary>
        /// 이전 레벨
        /// </summary>
        public int PreviousLevel { get; init; }

        /// <summary>
        /// 새 레벨
        /// </summary>
        public int NewLevel { get; init; }

        /// <summary>
        /// 전투력 변화량
        /// </summary>
        public int PowerChange { get; init; }

        /// <summary>
        /// 유저 데이터 변경분
        /// </summary>
        public UserDataDelta Delta { get; init; }
    }

    /// <summary>
    /// 캐릭터 레벨업 실패 이벤트
    /// </summary>
    public readonly struct CharacterLevelUpFailedEvent
    {
        public string CharacterInstanceId { get; init; }
        public int ErrorCode { get; init; }
        public string ErrorMessage { get; init; }
    }

    #endregion

    #region Ascension

    /// <summary>
    /// 캐릭터 돌파 완료 이벤트
    /// </summary>
    public readonly struct CharacterAscensionEvent
    {
        /// <summary>
        /// 캐릭터 인스턴스 ID
        /// </summary>
        public string CharacterInstanceId { get; init; }

        /// <summary>
        /// 이전 돌파 단계
        /// </summary>
        public int PreviousAscension { get; init; }

        /// <summary>
        /// 새 돌파 단계
        /// </summary>
        public int NewAscension { get; init; }

        /// <summary>
        /// 새 레벨 상한
        /// </summary>
        public int NewLevelCap { get; init; }

        /// <summary>
        /// 전투력 변화량
        /// </summary>
        public int PowerChange { get; init; }

        /// <summary>
        /// 유저 데이터 변경분
        /// </summary>
        public UserDataDelta Delta { get; init; }
    }

    /// <summary>
    /// 캐릭터 돌파 실패 이벤트
    /// </summary>
    public readonly struct CharacterAscensionFailedEvent
    {
        public string CharacterInstanceId { get; init; }
        public int ErrorCode { get; init; }
        public string ErrorMessage { get; init; }
    }

    #endregion
}
