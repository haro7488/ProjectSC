namespace Sc.Data
{
    /// <summary>
    /// 소모 가능한 아이템 인터페이스
    /// </summary>
    public interface IConsumable
    {
        /// <summary>
        /// 사용 시 소모량
        /// </summary>
        int ConsumeCount { get; }

        /// <summary>
        /// 쿨다운 (초)
        /// </summary>
        float Cooldown { get; }
    }
}
