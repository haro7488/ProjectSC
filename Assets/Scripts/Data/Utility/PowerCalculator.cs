namespace Sc.Data
{
    /// <summary>
    /// 캐릭터 전투력 계산기
    /// 공식: (HP/10) + (ATK*5) + (DEF*3) + (SPD*2) + (CritRate*100) + (CritDamage*50)
    /// </summary>
    public static class PowerCalculator
    {
        /// <summary>
        /// 스탯으로 전투력 계산
        /// </summary>
        public static int Calculate(CharacterStats stats)
        {
            return (int)(
                stats.HP / 10f +
                stats.ATK * 5 +
                stats.DEF * 3 +
                stats.SPD * 2 +
                stats.CritRate * 100 +
                stats.CritDamage * 50
            );
        }

        /// <summary>
        /// 캐릭터 정보로 전투력 계산
        /// </summary>
        public static int Calculate(
            OwnedCharacter character,
            CharacterData data,
            CharacterAscensionDatabase ascensionDb)
        {
            var stats = CalculateStats(character, data, ascensionDb);
            return Calculate(stats);
        }

        /// <summary>
        /// 캐릭터의 최종 스탯 계산
        /// </summary>
        public static CharacterStats CalculateStats(
            OwnedCharacter character,
            CharacterData data,
            CharacterAscensionDatabase ascensionDb)
        {
            // 레벨 보정 (레벨당 5% 증가)
            float levelMultiplier = 1f + (character.Level - 1) * 0.05f;

            var stats = new CharacterStats(
                (int)(data.BaseHp * levelMultiplier),
                (int)(data.BaseAtk * levelMultiplier),
                (int)(data.BaseDef * levelMultiplier),
                data.BaseSpd,
                data.CritRate,
                data.CritDamage
            );

            // 돌파 보너스 적용
            if (ascensionDb != null)
            {
                var ascensionBonus = ascensionDb.GetTotalStatBonus(data.Rarity, character.Ascension);
                stats = stats + ascensionBonus;
            }

            return stats;
        }

        /// <summary>
        /// 레벨업 후 전투력 변화량 미리보기
        /// </summary>
        public static int PreviewLevelUpPower(
            OwnedCharacter character,
            CharacterData data,
            CharacterAscensionDatabase ascensionDb,
            int targetLevel)
        {
            var preview = character;
            preview.Level = targetLevel;
            return Calculate(preview, data, ascensionDb);
        }

        /// <summary>
        /// 돌파 후 전투력 변화량 미리보기
        /// </summary>
        public static int PreviewAscensionPower(
            OwnedCharacter character,
            CharacterData data,
            CharacterAscensionDatabase ascensionDb,
            int targetAscension)
        {
            var preview = character;
            preview.Ascension = targetAscension;
            return Calculate(preview, data, ascensionDb);
        }
    }
}
