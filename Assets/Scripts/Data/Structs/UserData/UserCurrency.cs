using System;

namespace Sc.Data
{
    /// <summary>
    /// 유저 재화 데이터
    /// </summary>
    [Serializable]
    public struct UserCurrency
    {
        #region 기본 재화

        /// <summary>
        /// 골드 (일반 재화)
        /// </summary>
        public long Gold;

        /// <summary>
        /// 보석 (유료 재화)
        /// </summary>
        public int Gem;

        /// <summary>
        /// 무료 보석
        /// </summary>
        public int FreeGem;

        /// <summary>
        /// 현재 스태미나
        /// </summary>
        public int Stamina;

        /// <summary>
        /// 최대 스태미나
        /// </summary>
        public int MaxStamina;

        /// <summary>
        /// 스태미나 마지막 갱신 시간 (Unix Timestamp)
        /// </summary>
        public long StaminaUpdatedAt;

        #endregion

        #region 소환 재화

        /// <summary>
        /// 일반 소환권
        /// </summary>
        public int SummonTicket;

        /// <summary>
        /// 픽업 소환권
        /// </summary>
        public int PickupSummonTicket;

        #endregion

        #region 캐릭터 육성 재화

        /// <summary>
        /// 캐릭터 경험치 재료 (범용)
        /// </summary>
        public int CharacterExpMaterial;

        /// <summary>
        /// 돌파 재료 (범용)
        /// </summary>
        public int BreakthroughMaterial;

        /// <summary>
        /// 스킬북 (범용)
        /// </summary>
        public int SkillBook;

        #endregion

        #region 장비 육성 재화

        /// <summary>
        /// 장비 경험치 재료
        /// </summary>
        public int EquipmentExpMaterial;

        /// <summary>
        /// 강화석
        /// </summary>
        public int EnhanceStone;

        #endregion

        #region 컨텐츠 재화

        /// <summary>
        /// 아레나 입장권
        /// </summary>
        public int ArenaTicket;

        /// <summary>
        /// 아레나 코인 (상점 교환용)
        /// </summary>
        public int ArenaCoin;

        /// <summary>
        /// 길드 코인
        /// </summary>
        public int GuildCoin;

        /// <summary>
        /// 레이드 코인
        /// </summary>
        public int RaidCoin;

        #endregion

        #region 시즌 재화

        /// <summary>
        /// 시즌 코인 (시즌패스용)
        /// </summary>
        public int SeasonCoin;

        #endregion

        #region 계산 프로퍼티

        /// <summary>
        /// 총 보석 (유료 + 무료)
        /// </summary>
        public int TotalGem => Gem + FreeGem;

        #endregion

        #region 팩토리 메서드

        /// <summary>
        /// 기본값으로 초기화된 재화 생성
        /// </summary>
        public static UserCurrency CreateDefault()
        {
            return new UserCurrency
            {
                // 기본 재화
                Gold = 10000,
                Gem = 0,
                FreeGem = 1000,
                Stamina = 100,
                MaxStamina = 100,
                StaminaUpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),

                // 소환 재화
                SummonTicket = 10,
                PickupSummonTicket = 0,

                // 캐릭터 육성 재화
                CharacterExpMaterial = 0,
                BreakthroughMaterial = 0,
                SkillBook = 0,

                // 장비 육성 재화
                EquipmentExpMaterial = 0,
                EnhanceStone = 0,

                // 컨텐츠 재화
                ArenaTicket = 5,
                ArenaCoin = 0,
                GuildCoin = 0,
                RaidCoin = 0,

                // 시즌 재화
                SeasonCoin = 0
            };
        }

        #endregion

        #region 재화 조회 메서드

        /// <summary>
        /// CostType에 해당하는 재화 수량 조회
        /// </summary>
        /// <param name="costType">재화 타입</param>
        /// <returns>보유 수량</returns>
        public long GetAmount(CostType costType)
        {
            return costType switch
            {
                // 기본 재화
                CostType.Gold => Gold,
                CostType.Gem => TotalGem,
                CostType.Stamina => Stamina,

                // 소환 재화
                CostType.SummonTicket => SummonTicket,
                CostType.PickupSummonTicket => PickupSummonTicket,

                // 캐릭터 육성 재화
                CostType.CharacterExp => CharacterExpMaterial,
                CostType.BreakthroughMaterial => BreakthroughMaterial,
                CostType.SkillBook => SkillBook,

                // 장비 육성 재화
                CostType.EquipmentExp => EquipmentExpMaterial,
                CostType.EnhanceStone => EnhanceStone,

                // 컨텐츠 재화
                CostType.ArenaTicket => ArenaTicket,
                CostType.ArenaCoin => ArenaCoin,
                CostType.GuildCoin => GuildCoin,
                CostType.RaidCoin => RaidCoin,

                // 시즌 재화
                CostType.SeasonCoin => SeasonCoin,

                // 무료 / 이벤트 재화
                CostType.None => long.MaxValue,
                CostType.EventCurrency => 0, // EventCurrencyData에서 별도 조회 필요

                _ => 0
            };
        }

        /// <summary>
        /// 재화 차감 가능 여부 확인
        /// </summary>
        /// <param name="costType">재화 타입</param>
        /// <param name="amount">필요 수량</param>
        /// <returns>차감 가능 여부</returns>
        public bool CanAfford(CostType costType, int amount)
        {
            if (costType == CostType.EventCurrency)
            {
                // 이벤트 재화는 EventCurrencyData에서 별도 확인 필요
                return false;
            }
            return GetAmount(costType) >= amount;
        }

        /// <summary>
        /// 여러 비용 동시 확인
        /// </summary>
        /// <param name="costs">비용 목록 (타입, 수량)</param>
        /// <returns>모든 비용 차감 가능 여부</returns>
        public bool CanAffordMultiple(params (CostType type, int amount)[] costs)
        {
            foreach (var (type, amount) in costs)
            {
                if (!CanAfford(type, amount)) return false;
            }
            return true;
        }

        #endregion
    }
}
