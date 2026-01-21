namespace Sc.Core
{
    /// <summary>
    /// 배지 카운트 제공자 인터페이스
    /// 각 Contents에서 구현하여 BadgeManager에 등록
    /// </summary>
    public interface IBadgeProvider
    {
        BadgeType Type { get; }
        int CalculateBadgeCount();
    }
}
