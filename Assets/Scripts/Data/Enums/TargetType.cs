namespace Sc.Data
{
    /// <summary>
    /// 스킬 대상 유형
    /// </summary>
    public enum TargetType
    {
        SingleEnemy,  // 적 단일
        AllEnemy,     // 적 전체
        SingleAlly,   // 아군 단일
        AllAlly,      // 아군 전체
        Self          // 자기 자신
    }
}
