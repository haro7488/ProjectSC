namespace Sc.Data
{
    /// <summary>
    /// 재화/비용 유형
    /// </summary>
    public enum CostType
    {
        // === 기본 재화 ===
        None = 0,           // 무료
        Gold = 1,           // 골드 (일반 재화)
        Gem = 2,            // 보석 (프리미엄 재화, FreeGem 우선 소비)
        Stamina = 3,        // 스태미나

        // === 소환 재화 ===
        SummonTicket = 10,        // 일반 소환권
        PickupSummonTicket = 11,  // 픽업 소환권

        // === 캐릭터 육성 재화 ===
        CharacterExp = 20,        // 캐릭터 경험치 재료
        BreakthroughMaterial = 21,// 돌파 재료 (범용)
        SkillBook = 22,           // 스킬북 (범용)

        // === 장비 육성 재화 ===
        EquipmentExp = 30,        // 장비 경험치 재료
        EnhanceStone = 31,        // 강화석

        // === 컨텐츠 재화 ===
        ArenaTicket = 40,         // 아레나 입장권
        ArenaCoin = 41,           // 아레나 코인 (상점용)
        GuildCoin = 42,           // 길드 코인 (상점용)
        RaidCoin = 43,            // 레이드 코인

        // === 시즌/이벤트 재화 ===
        SeasonCoin = 50,          // 시즌 코인 (시즌패스)
        EventCurrency = 100,      // 이벤트 재화 (동적, EventCurrencyData 참조)
    }
}
