namespace Sc.Data
{
    /// <summary>
    /// 리셋/제한 타입.
    /// 구매 제한, 퀘스트 리셋, 이벤트 기간 등에 사용.
    /// </summary>
    public enum LimitType
    {
        /// <summary>
        /// 제한 없음 (무제한)
        /// </summary>
        None = 0,

        /// <summary>
        /// 일일 리셋 (매일 UTC 00:00)
        /// </summary>
        Daily = 1,

        /// <summary>
        /// 주간 리셋 (매주 월요일 UTC 00:00)
        /// </summary>
        Weekly = 2,

        /// <summary>
        /// 월간 리셋 (매월 1일 UTC 00:00)
        /// </summary>
        Monthly = 3,

        /// <summary>
        /// 영구 제한 (계정당 1회, 리셋 없음)
        /// </summary>
        Permanent = 4,

        /// <summary>
        /// 이벤트 기간 제한 (이벤트 종료 시 리셋)
        /// </summary>
        EventPeriod = 5,
    }
}
