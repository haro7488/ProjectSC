namespace Sc.Data
{
    /// <summary>
    /// 아이템 공통 속성 인터페이스
    /// </summary>
    public interface IThing
    {
        string Id { get; }
        string Name { get; }
        string NameEn { get; }
        string Description { get; }
        Rarity Rarity { get; }
    }
}
