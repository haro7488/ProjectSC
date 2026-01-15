namespace Sc.Packet
{
    /// <summary>
    /// API 요청 인터페이스
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// 요청 타임스탬프 (Unix Timestamp)
        /// </summary>
        long Timestamp { get; }
    }
}
