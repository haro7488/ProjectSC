namespace Sc.Data
{
    /// <summary>
    /// 수량 관리 가능한 아이템 인터페이스
    /// </summary>
    public interface IStackable
    {
        /// <summary>
        /// 최대 스택 수량
        /// 0 = 스택 불가, -1 = 무제한
        /// </summary>
        int MaxStackCount { get; }
    }
}
