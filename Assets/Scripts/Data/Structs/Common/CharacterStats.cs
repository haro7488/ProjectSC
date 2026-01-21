using System;

namespace Sc.Data
{
    /// <summary>
    /// 캐릭터 스탯 구조체
    /// </summary>
    [Serializable]
    public struct CharacterStats
    {
        public int HP;
        public int ATK;
        public int DEF;
        public int SPD;
        public float CritRate;
        public float CritDamage;

        public CharacterStats(int hp, int atk, int def, int spd, float critRate, float critDamage)
        {
            HP = hp;
            ATK = atk;
            DEF = def;
            SPD = spd;
            CritRate = critRate;
            CritDamage = critDamage;
        }

        public static CharacterStats operator +(CharacterStats a, CharacterStats b)
        {
            return new CharacterStats(
                a.HP + b.HP,
                a.ATK + b.ATK,
                a.DEF + b.DEF,
                a.SPD + b.SPD,
                a.CritRate + b.CritRate,
                a.CritDamage + b.CritDamage
            );
        }

        public override string ToString()
        {
            return $"HP:{HP} ATK:{ATK} DEF:{DEF} SPD:{SPD} Crit:{CritRate:P1}/{CritDamage:P0}";
        }
    }
}
